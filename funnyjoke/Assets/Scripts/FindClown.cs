using UnityEngine;

public class FindClown : MonoBehaviour
{
    [SerializeField] private float northThreshold;
    [SerializeField] private float southThreshold;
    [SerializeField] private float eastThreshold;
    [SerializeField] private float westThreshold;
    
    private AI_Manager aiManager;

    private void Start()
    {
        aiManager = AI_Manager.instance;
    }

    public Direction GetClownDirection()
    {
        var direction = Direction.Central;

        var clown = aiManager.clown;
        
        if (clown.transform.position.z > northThreshold)
            direction = Direction.North;
        else if (clown.transform.position.z < southThreshold)
            direction = Direction.South;
        else if (clown.transform.position.x > eastThreshold)
            direction = Direction.East;
        else if (clown.transform.position.x < westThreshold)
            direction = Direction.West;
        
        if (clown.transform.position.x > eastThreshold && direction != Direction.East)
            direction.AddDirection(Direction.East);
        else if (clown.transform.position.x < westThreshold && direction != Direction.West)
            direction.AddDirection(Direction.West);
        
        return direction;
    }
    
    
}
