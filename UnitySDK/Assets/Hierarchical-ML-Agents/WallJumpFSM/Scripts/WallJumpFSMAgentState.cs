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

}
