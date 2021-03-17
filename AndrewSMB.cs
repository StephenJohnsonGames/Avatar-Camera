using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndrewSMB : StateMachineBehaviour {

    public delegate void AndrewSMBEvent();
    public static AndrewSMBEvent OnStandupEnded;
    public static AndrewSMBEvent OnSitdown;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if ((stateInfo.IsName("Sit") || stateInfo.IsName("Unarmed-Sit-Standup")) && OnStandupEnded != null) {
            OnStandupEnded();
        }

        if (stateInfo.IsName("Unarmed-Sit-Sitdown") || stateInfo.IsName("Unarmed-Sit-Idle") && OnSitdown != null) {

            OnSitdown();
        }


    
    }

}
