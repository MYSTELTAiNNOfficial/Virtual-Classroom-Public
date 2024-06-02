using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] LineRenderer infoLineRend;
    [SerializeField] GameObject pointBreakLineRend;
    [SerializeField] GameObject pointBreakLineRend2;
    [SerializeField] GameObject pointBreakLineRend3;

    [SerializeField] int lineRendPosCount;
    GameObject targetObject;
    bool isPoint3Enabled;

    // Start is called before the first frame update
    void Start()
    {
        infoLineRend.positionCount = lineRendPosCount;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObject != null)
        {
            infoLineRend.gameObject.SetActive(true);
            infoLineRend.SetPosition(0, pointBreakLineRend.transform.position);
            infoLineRend.SetPosition(1, pointBreakLineRend2.transform.position);
            infoLineRend.SetPosition(2, targetObject.transform.position);
            if (isPoint3Enabled)
            {
                infoLineRend.SetPosition(3, pointBreakLineRend3.transform.position);
            }
            else
            {
                infoLineRend.SetPosition(3, pointBreakLineRend2.transform.position);
            }
        }
        else
        {
            infoLineRend.gameObject.SetActive(false);
        }
    }

    public void SetTargetforLine(GameObject go) { targetObject = go; }

    public void RemoveTarget() {  targetObject = null; }

    public void SetIsPoint3EnabledState(bool val)
    {
        isPoint3Enabled = val;
    }
}
