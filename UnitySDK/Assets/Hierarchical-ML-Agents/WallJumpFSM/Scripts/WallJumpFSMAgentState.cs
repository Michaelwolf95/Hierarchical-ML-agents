using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using HutongGames.PlayMaker;

public class WallJumpFSMAgentState : FsmStateAction
{
    [RequiredField] [CheckForComponent(typeof(Brain))] 
    [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
    public FsmGameObject brainGameObject;
    
    public WallJumpFSMAgent mlAgent;

    public Brain brain;

    public override void Reset()
    {
        base.Reset();
        mlAgent = null;
        brain = null;
    }

    public override void Awake()
    {
        base.Awake();
        if (mlAgent == null)
        {
            mlAgent = Owner.gameObject.GetComponent<WallJumpFSMAgent>();
        }
        if (brain == null && brainGameObject != null)
        {
            brain = brainGameObject.Value.GetComponent<Brain>();
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
        if (brain == null && brainGameObject.Value != null)
        {
            brain = brainGameObject.Value.GetComponent<Brain>();
        }
        //Debug.Log("Entered state with brain: " + brain.gameObject.name);
        if (mlAgent != null)
        {
            mlAgent.GiveBrain(brain);
            mlAgent.OnAgentAction += DoAgentAction;
            mlAgent.OnCollectObservations += DoCollectObservations;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        //Debug.Log("Exiting state with brain: " + brain.gameObject.name);
        if (mlAgent != null)
        {
            mlAgent.GiveBrain(null);
            mlAgent.OnAgentAction -= DoAgentAction;
            mlAgent.OnCollectObservations -= DoCollectObservations;
        }
    }
    
    private void DoCollectObservations()
    {
        if (mlAgent != null)
        {
            mlAgent.CollectPhysicalObservations(); // Original observations
        }
    }

    private void DoAgentAction(float[] vectorAction, string textAction)
    {
        if (mlAgent != null)
        {
            mlAgent.MoveAgent(vectorAction);
        }
    }
    
//    public GameObject ground;
//    public GameObject spawnArea;
//    Bounds spawnAreaBounds;
//
//
//    public GameObject goal;
//    public GameObject shortBlock;
//    public GameObject wall;
//    Rigidbody shortBlockRB;
//    Rigidbody agentRB;
//    Material groundMaterial;
//    Renderer groundRenderer;
//    WallJumpFSMAcademy academy;
//    RayPerception rayPer;
//
//    public float jumpingTime;
//    public float jumpTime;
//    // This is a downward force applied when falling to make jumps look
//    // less floaty
//    public float fallingForce;
//    // Use to check the coliding objects
//    public Collider[] hitGroundColliders = new Collider[3];
//    Vector3 jumpTargetPos;
//    Vector3 jumpStartingPos;
//
//    string[] detectableObjects;
//
//
//    // Begin the jump sequence
//    public void Jump()
//    {
//
//        jumpingTime = 0.2f;
//        jumpStartingPos = agentRB.position;
//    }
//
//    /// <summary>
//    /// Does the ground check.
//    /// </summary>
//    /// <returns><c>true</c>, if the agent is on the ground, 
//    /// <c>false</c> otherwise.</returns>
//    /// <param name="boxWidth">The width of the box used to perform 
//    /// the ground check. </param>
//    public bool DoGroundCheck(bool smallCheck)
//    {
//        if (!smallCheck)
//        {
//            hitGroundColliders = new Collider[3];
//            Physics.OverlapBoxNonAlloc(
//                gameObject.transform.position + new Vector3(0, -0.05f, 0),
//                new Vector3(0.95f / 2f, 0.5f, 0.95f / 2f),
//                hitGroundColliders,
//                gameObject.transform.rotation);
//            bool grounded = false;
//            foreach (Collider col in hitGroundColliders)
//            {
//
//                if (col != null && col.transform != this.transform &&
//                    (col.CompareTag("walkableSurface") ||
//                     col.CompareTag("block") ||
//                     col.CompareTag("wall")))
//                {
//                    grounded = true; //then we're grounded
//                    break;
//                }
//            }
//            return grounded;
//        }
//        else
//        {
//
//            RaycastHit hit;
//            Physics.Raycast(transform.position + new Vector3(0, -0.05f, 0), -Vector3.up, out hit,
//                1f);
//
//            if (hit.collider != null &&
//                (hit.collider.CompareTag("walkableSurface") ||
//                 hit.collider.CompareTag("block") ||
//                 hit.collider.CompareTag("wall"))
//                && hit.normal.y > 0.95f)
//            {
//                return true;
//            }
//
//            return false;
//        }
//    }
//
//
//    /// <summary>
//    /// Moves  a rigidbody towards a position smoothly.
//    /// </summary>
//    /// <param name="targetPos">Target position.</param>
//    /// <param name="rb">The rigidbody to be moved.</param>
//    /// <param name="targetVel">The velocity to target during the
//    ///  motion.</param>
//    /// <param name="maxVel">The maximum velocity posible.</param>
//    void MoveTowards(
//        Vector3 targetPos, Rigidbody rb, float targetVel, float maxVel)
//    {
//        Vector3 moveToPos = targetPos - rb.worldCenterOfMass;
//        Vector3 velocityTarget = moveToPos * targetVel * Time.fixedDeltaTime;
//        if (float.IsNaN(velocityTarget.x) == false)
//        {
//            rb.velocity = Vector3.MoveTowards(
//                rb.velocity, velocityTarget, maxVel);
//        }
//    }
//    
//    public void MoveAgent(float[] act)
//    {
//
//        AddReward(-0.0005f);
//        bool smallGrounded = DoGroundCheck(true);
//        bool largeGrounded = DoGroundCheck(false);
//
//        Vector3 dirToGo = Vector3.zero;
//        Vector3 rotateDir = Vector3.zero;
//        int dirToGoForwardAction = (int)act[0];
//        int rotateDirAction = (int)act[1];
//        int dirToGoSideAction = (int)act[2];
//        int jumpAction = (int)act[3];
//
//        if (dirToGoForwardAction == 1)
//            dirToGo = transform.forward * 1f * (largeGrounded ? 1f : 0.5f);
//        else if (dirToGoForwardAction == 2)
//            dirToGo = transform.forward * -1f * (largeGrounded ? 1f : 0.5f);
//        if (rotateDirAction == 1)
//            rotateDir = transform.up * -1f;
//        else if (rotateDirAction == 2)
//            rotateDir = transform.up * 1f;
//        if (dirToGoSideAction == 1)
//            dirToGo = transform.right * -0.6f * (largeGrounded ? 1f : 0.5f);
//        else if (dirToGoSideAction == 2)
//            dirToGo = transform.right * 0.6f * (largeGrounded ? 1f : 0.5f);
//        if (jumpAction == 1)
//            if ((jumpingTime <= 0f) && smallGrounded)
//            {
//                Jump();
//            }
//
//        transform.Rotate(rotateDir, Time.fixedDeltaTime * 300f);
//        agentRB.AddForce(dirToGo * academy.agentRunSpeed,
//            ForceMode.VelocityChange);
//
//        if (jumpingTime > 0f)
//        {
//            jumpTargetPos =
//                new Vector3(agentRB.position.x,
//                    jumpStartingPos.y + academy.agentJumpHeight,
//                    agentRB.position.z) + dirToGo;
//            MoveTowards(jumpTargetPos, agentRB, academy.agentJumpVelocity,
//                academy.agentJumpVelocityMaxChange);
//
//        }
//
//        if (!(jumpingTime > 0f) && !largeGrounded)
//        {
//            agentRB.AddForce(
//                Vector3.down * fallingForce, ForceMode.Acceleration);
//        }
//        jumpingTime -= Time.fixedDeltaTime;
//    }

}
