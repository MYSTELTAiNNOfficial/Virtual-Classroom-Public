using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModelGigiController : MonoBehaviour
{
    GameObject modelGigiTemp;
    [SerializeField] GameObject assesmentStateTMP;
    [SerializeField] GameObject modelGigiPrefab;
    [SerializeField] GameObject modelGigiSpawn;
    ModelObjectController modelObjectController;

    private void Start()
    {
        modelGigiSpawn = GameObject.FindGameObjectWithTag("ModelGigiSpawn");
    }

    private void Update()
    {
        modelObjectController = GameObject.FindGameObjectWithTag("ModelGigi").GetComponent<ModelObjectController>();
        assesmentStateTMP.GetComponent<TextMeshProUGUI>().text = $"Assessment: {modelObjectController.GetAssesmentState()}";
    }

    public void SummonModelGigi()
    {
        Instantiate(modelGigiPrefab, modelGigiSpawn.transform);
        modelGigiTemp = GameObject.FindGameObjectWithTag("ModelGigi");
        modelGigiTemp.transform.parent = null;
        modelGigiTemp.transform.position = modelGigiSpawn.transform.position;
        modelObjectController = modelGigiTemp.GetComponent<ModelObjectController>();
    }

    public void ChangeAssesmentState()
    {
        if (modelObjectController == null)
        {
            modelObjectController = GameObject.FindGameObjectWithTag("ModelGigi").GetComponent<ModelObjectController>();
        }
        modelObjectController.SetAssesmentState();
    }

    public void RemoveModelGigi()
    {
        Destroy(modelGigiTemp);
        modelGigiTemp = null;
    }
}
