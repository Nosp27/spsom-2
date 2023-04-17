using System.Collections;
using System.Collections.Generic;
using GameControl.StateMachine;
using UnityEngine;
using UnityEngine.UI;

public class StateMachineVisualizer : MonoBehaviour
{
    [SerializeField] private PhysicalMovement _p;
    private StateMachine sm;
    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private Transform uiContainer;

    
    private Dictionary<LambdaState, GameObject> states;
    private LambdaState selected;

    IEnumerator Start()
    {
        do
        {
            yield return 0;
            sm = _p.stateMachine;
        } while (sm == null);

        states = new Dictionary<LambdaState, GameObject>();
        foreach (var state in sm.GetAllStates())
        {
            GameObject obj = Instantiate(uiPrefab, uiContainer);
            obj.SetActive(true);
            LambdaState ls = state as LambdaState;
            if (ls == null)
                continue;
            obj.GetComponentInChildren<Text>().text = ls.name;
            states[ls] = obj;
        }
    }
    
    void Update()
    {
        if (sm == null)
            return;
        
        LambdaState actualSelected = sm.CurrentState as LambdaState;
        if (actualSelected == selected)
            return;
        Select(actualSelected);
    }

    void Select(LambdaState s)
    {
        Deselect();
        GameObject go = states[s];
        go.GetComponent<Image>().color = Color.green;
        selected = s;
    }

    void Deselect()
    {
        if (selected == null)
            return;
        GameObject go = states[selected];
        go.GetComponent<Image>().color = Color.black;
        selected = null;
    }
}
