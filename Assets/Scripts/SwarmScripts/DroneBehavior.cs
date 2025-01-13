using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DroneBehavior : MonoBehaviour {
	
	// the overall speed of the simulation
	public float speed = 0.10f;
	// max speed any particular drone can move at
	public float maxSpeed = 0.20f;
	// maximum steering power
	public float maxSteer = .0005f;
	
	// weights: used to modify the drone's movement
	public float separationWeight = 1f;
	public float alignmentWeight = 1f;
	public float cohesionWeight = 1f;
	public float boundsWeight = 1f;
	
	public float neighborRadius = 0.5f;
	public float desiredSeparation = 0.06f;
	
	// velocity influences
	public Vector3 _separation;
	public Vector3 _alignment;
	public Vector3 _cohesion;
	public Vector3 _bounds;
	
	// other members of my swarm
	public List<GameObject> drones;
	
	public SwarmBehavior swarm;
	
	private Vector3 prevpos = new Vector3 (0f,0f,0f);
	
	void FixedUpdate()
	{
		// we should always apply physics forces in FixedUpdate
		Flock();

		transform.rotation = Quaternion.LookRotation (  prevpos - transform.position  );
		prevpos = prevpos*0.8f+transform.position*0.2f;
	}
	
	void Start()
	{

	}
	
	protected virtual void Update()
	{

	}
	
	public virtual void Flock()
	{
		
		Vector3 newVelocity = Vector3.zero;
		
		CalculateVelocities();
		
		//transform.forward = _alignment;
		newVelocity += _separation * separationWeight;
		newVelocity += _alignment * alignmentWeight;
		newVelocity += _cohesion * cohesionWeight;
		newVelocity += _bounds * boundsWeight;
		newVelocity = newVelocity * speed;
		newVelocity = GetComponent<Rigidbody>().velocity + newVelocity;
		newVelocity.y = 0f;

        GetComponent<Rigidbody>().velocity = Limit(newVelocity, maxSpeed);
	}
	
	/// <summary>
	/// Calculates the influence velocities for the drone. We do this in one big loop for efficiency.
	/// </summary>
	protected virtual void CalculateVelocities()
	{
		// the general procedure is that we add up velocities based on the neighbors in our radius for a particular influence (cohesion, separation, etc.) 
		// and divide the sum by the total number of drones in our neighbor radius
		// this produces an evened-out velocity that is aligned with its neighbors to apply to the target drone		
		Vector3 separationSum = Vector3.zero;
		Vector3 alignmentSum = Vector3.zero;
		Vector3 cohesionSum = Vector3.zero;
		Vector3 boundsSum = Vector3.zero;
		
		int separationCount = 0;
		int alignmentCount = 0;
		int cohesionCount = 0;
		int boundsCount = 0;
		
		for (int i = 0; i < this.drones.Count; i++)
		{
			if (drones[i] == null) continue;
			
			float distance = Vector3.Distance(transform.position, drones[i].transform.position);
			
			// separation
			// calculate separation influence velocity for this drone, based on its preference to keep distance between itself and neighboring drones
			if (distance > 0 && distance < desiredSeparation)
			{
				// calculate vector headed away from myself
				Vector3 direction = transform.position - drones[i].transform.position;	
				direction.Normalize();
				direction = direction / distance; // weight by distance
				separationSum += direction;
				separationCount++;
			}
			
			// alignment & cohesion
			// calculate alignment influence vector for this drone, based on its preference to be aligned with neighboring drones
			// calculate cohesion influence vector for this drone, based on its preference to be close to neighboring drones
			if (distance > 0 && distance < neighborRadius)
			{
				alignmentSum += drones[i].GetComponent<Rigidbody>().velocity;
				alignmentCount++;
				
				cohesionSum += drones[i].transform.position;
				cohesionCount++;
			}
			
			// bounds
			// calculate the bounds influence vector for this drone, based on whether or not neighboring drones are in bounds
			Bounds bounds = new Bounds(swarm.transform.position, new Vector3(swarm.swarmBounds.x, 10000f, swarm.swarmBounds.y));
			if (distance > 0 && distance < neighborRadius && !bounds.Contains(drones[i].transform.position))
			{
				Vector3 diff = transform.position - swarm.transform.position;
				if (diff.magnitude> 0)
				{
					boundsSum += swarm.transform.position;
					boundsCount++;
				}
			}
		}
		
		// end
		_separation = separationCount > 0 ? separationSum / separationCount : separationSum;
		_separation.y = 0;
		_alignment = alignmentCount > 0 ? Limit(alignmentSum / alignmentCount, maxSteer) : alignmentSum;
		_alignment.y = 0;
		_cohesion = cohesionCount > 0 ? Steer(cohesionSum / cohesionCount, false) : cohesionSum;
		_cohesion.y = 0;
		_bounds = boundsCount > 0 ? Steer(boundsSum / boundsCount, false) : boundsSum;
		_bounds.y = 0;
	}
	
	/// <summary>
	/// Returns a steering vector to move the drone towards the target
	/// </summary>
	/// <param type="Vector3" name="target"></param>
	/// <param type="bool" name="slowDown"></param>
	protected virtual Vector3 Steer(Vector3 target, bool slowDown)
	{
		// the steering vector
		Vector3 steer = Vector3.zero;
		Vector3 targetDirection = target - transform.position;
		targetDirection.y = 0;
		float targetDistance = targetDirection.magnitude;
		
		transform.LookAt(target);
		
		if (targetDistance > 0)
		{
			// move towards the target
			targetDirection.Normalize();
			
			// we have two options for speed
			if (slowDown && targetDistance < 1.0f * speed)
			{
				targetDirection *= (maxSpeed * targetDistance / (1.0f * speed));
				targetDirection *= speed;
			}
			else
			{
				targetDirection *= maxSpeed;
			}
			
			// set steering vector
			steer = targetDirection - GetComponent<Rigidbody>().velocity;
			steer = Limit(steer, maxSteer);
		}
		
		return steer;
	}
	
	/// <summary>
	/// Limit the magnitude of a vector to the specified max
	/// </summary>
	/// <param type="Vector3" name="v"></param>
	/// <param type="float" name="max"></param>
	protected virtual Vector3 Limit(Vector3 v, float max)
	{
		if (v.magnitude > max)
		{
			return v.normalized * max;
		}
		else
		{
			return v;
		}
	}
	
	/// <summary>
	/// Show some gizmos to provide a visual indication of what is happening: white => alignment, magenta => separation, blue => cohesion
	/// </summary>
	protected virtual void OnDrawGizmosSelected()
	{
		//Gizmos.color = Color.green;
		//Gizmos.DrawWireSphere(transform.position, neighborRadius);
		
		Gizmos.color = Color.white;
		Gizmos.DrawLine(transform.position, transform.position + _alignment.normalized * neighborRadius);
		
		Gizmos.color = Color.magenta;
		Gizmos.DrawLine(transform.position, transform.position + _separation.normalized * neighborRadius);
		
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + _cohesion.normalized * neighborRadius);
	}
}

