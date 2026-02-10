using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Unity.Cinemachine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "CatchPlayer", story: "[Agent] jump scares [Target] and resets", category: "Action", id: "b126dfa57ff81a73675edf16bf7cf02f")]
public partial class CatchPlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> agent;
    [SerializeReference] public BlackboardVariable<GameObject> target;

    protected override Status OnStart()
    {
        GameObject enemy = agent.Value;
        
        if (!enemy) return Status.Failure;

        Animator anim = enemy.GetComponentInChildren<Animator>();
        
        var camComponent = enemy.GetComponentInChildren<CinemachineCamera>(true);

        if (camComponent)
        {
            JumpScareManager.Instance.TriggerScare(camComponent.gameObject, anim,() => {
                UnityEngine.Object.FindFirstObjectByType<MazeGenerator>().RespawnPlayer();
            });
            return Status.Success;
        }
    
        return Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}