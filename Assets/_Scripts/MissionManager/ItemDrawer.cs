using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GhostBoy.Mission;

namespace GhostBoy.Mission
{
    public class ItemDrawer : MonoBehaviour
    {
        [SerializeField] private TMP_Text countTxt;
        [SerializeField] private TMP_Text nameTxt;
        [SerializeField] private TMP_Text infoTxt;
        [SerializeField] private Image bar;

        public void SetValues(Mission mission)
        {
            if(countTxt)
                countTxt.text = $"{mission.index.DivideBy(mission.Count()) * 100} %";

            if (nameTxt)
                nameTxt.text = mission.missionName;
                
            if(infoTxt)
                infoTxt.text = mission.Info();

            if(bar)
                bar.fillAmount = mission.Progress();
        }
    }
}
