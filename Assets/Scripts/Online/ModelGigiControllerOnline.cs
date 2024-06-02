using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModelGigiControllerOnline : NetworkBehaviour
{
    GameObject modelGigiTemp;
    [SerializeField] GameObject assesmentStateTMP;
    [SerializeField] GameObject restrictedStateTMP;
    [SerializeField] GameObject modelGigiPrefab;
    [SerializeField] GameObject modelGigiSpawn;
    ModelObjectControllerOnline modelObjectControllerOnline;

    private void Start()
    {
        SearchModelGigiSpawn();
    }

    private void Update()
    {
        modelObjectControllerOnline = GameObject.FindGameObjectWithTag("ModelGigi").GetComponent<ModelObjectControllerOnline>();
        restrictedStateTMP.GetComponent<TextMeshProUGUI>().text = $"Restriction: {modelObjectControllerOnline.GetRestrictedState()}";
        assesmentStateTMP.GetComponent<TextMeshProUGUI>().text = $"Assessment: {modelObjectControllerOnline.GetAssessmentState()}";
    }

    public void SearchModelGigiSpawn()
    {
        modelGigiSpawn = GameObject.FindGameObjectWithTag("ModelGigiSpawn");
    }

    [ServerRpc(RequireOwnership = false)]
    public void SummonModelGigiServerRpc()
    {
        if (modelGigiSpawn == null)
        {
            SearchModelGigiSpawn();
        }
        modelGigiTemp = Instantiate(modelGigiPrefab, modelGigiSpawn.transform);
        modelGigiTemp.transform.GetComponent<NetworkObject>().Spawn();
        modelGigiTemp.transform.parent = null;
        modelGigiTemp.transform.position = modelGigiSpawn.transform.position;
        modelObjectControllerOnline = modelGigiTemp.GetComponent<ModelObjectControllerOnline>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeAssesmentStateServerRpc()
    {
        if (modelObjectControllerOnline == null)
        {
            modelObjectControllerOnline = GameObject.FindGameObjectWithTag("ModelGigi").GetComponent<ModelObjectControllerOnline>();
        }
        modelObjectControllerOnline.ToggleSetAssesmentState();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeRestricedStateServerRpc()
    {
        if (modelObjectControllerOnline == null)
        {
            modelObjectControllerOnline = GameObject.FindGameObjectWithTag("ModelGigi").GetComponent<ModelObjectControllerOnline>();
        }
        modelObjectControllerOnline.ToggleSetRestrictedState();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveModelGigiServerRpc()
    {
        Destroy(modelGigiTemp);
        modelGigiTemp = null;
    }

    public void SummonModelGigi()
    {
        SummonModelGigiServerRpc();
    }

    public void ChangeAssesmentState()
    {
        ChangeAssesmentStateServerRpc();
    }

    public void ChangRestrictedState()
    {
        ChangeRestricedStateServerRpc();
    }

    public void RemoveModelGigi()
    {
        RemoveModelGigiServerRpc();
    }
}
