using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    private Vector3 startingPosition;


    public void Start()
    {
        startingPosition = transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(Random.Range(0.0f, +9f), 1, Random.Range(+5f, +6f));
        targetTransform.localPosition = new Vector3(Random.Range(-2f, -9f), 1, Random.Range(+6f, -6f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float moveSpeed = 5.0f;

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Goal")
        {
            Debug.Log("Agent Collided with Goal");
            SetReward(+1.0f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }

        if (other.transform.name == "Wall")
        {
            Debug.Log("Agent Collided with Wall");
            SetReward(-1.0f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }

        // Save for when wanting to increment or decrement
        // reward points
        //AddReward();
    }
}