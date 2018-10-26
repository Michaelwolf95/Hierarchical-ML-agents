using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using HutongGames.PlayMaker;

public class WallJumpFSMAgent_DeciderState : FsmStateAction
{
	[RequiredField] [CheckForComponent(typeof(Brain))] 
	[HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
	public FsmGameObject brainGameObject;

	public FsmInt choiceIndex;

	public GameObject wallObject; //Check the height value.
    
	public WallJumpFSMAgent mlAgent;

	public Brain brain;

	public override void Reset()
	{
		base.Reset();
		mlAgent = null;
		brain = null;
		wallObject = null;
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
		Debug.Log("Entered Decider State");
		
		if (brain == null && brainGameObject.Value != null)
		{
			brain = brainGameObject.Value.GetComponent<Brain>();
			if (brain.gameObject.activeInHierarchy == false)
			{
				Debug.Log("Decider Brain not active.");
			}
		}
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
		if (mlAgent != null)
		{
			mlAgent.GiveBrain(null);
			mlAgent.OnAgentAction -= DoAgentAction;
			mlAgent.OnCollectObservations -= DoCollectObservations;
		}

		choiceIndex.Value = -1;
	}
	
	private void DoCollectObservations()
	{
		if (mlAgent != null)
		{
			//mlAgent.CollectPhysicalObservations(); // Original observations
			float perceivedHeight = wallObject.transform.localScale.y;
			
			mlAgent.AddVectorObs(perceivedHeight);
		}
	}

	private void DoAgentAction(float[] vectorAction, string textAction)
	{
		if (mlAgent != null)
		{
			int choice = (int) vectorAction[0];

			choiceIndex.Value = choice;
			//mlAgent.MoveAgent(vectorAction);

		}
	}
}
