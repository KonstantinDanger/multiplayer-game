using Mirror;
using UnityEngine;

public class PatrolAction : AIAction
{
    private Vector3 _patrolDestination = Vector3.zero;

    public override void Execute(Enemy self, NetworkBehaviour target)
    {
        //if (distanceToPoint < 0.1f)
        {
            //start patrol idle timer 
            //reset current patrol destination point
            //score (weight) should be 0.0f
            //return;
        }

        //if (idle patrol timer pass)
        {
            //if (point != Vector3.zero)
            //return;

            //PickRandomDestinationPoint();
        }
        //else
        {
            //return
        }

        //go to destination point
    }

    private Vector3 PickRandomDestinationPoint()
        => _patrolDestination * UnityEngine.Random.Range(-1, 1);
}
