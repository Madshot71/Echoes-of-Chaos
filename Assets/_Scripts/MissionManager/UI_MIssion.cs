using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using GhostBoy.Mission;
using TMPro;

public class UI_MIssion : MonoBehaviour 
{
    [SerializeField] private MissionManager manager;
    private List<MissionItem> ui_missions = new List<MissionItem>();
    [SerializeField] private ItemDrawer Prefab;
    [SerializeField] private RectTransform content; 

    public void Start()
    {
        foreach(var item in manager.Missions)
        {
            ui_missions.Add(new MissionItem(item , CreateDrawer()));
        }
    }

    private void LateUpdate()
    {
        foreach(var item in ui_missions)
        {
            item.Update();
        }
    }

    private ItemDrawer CreateDrawer()
    {
        return Instantiate(Prefab, content);
    }

    protected class MissionItem
    {
        public string name;
        internal Mission mission;

        public ItemDrawer drawer;

        public MissionItem(Mission mission, ItemDrawer drawer)
        {
            this.drawer = drawer;
            this.mission = mission;
        }

        public void Update()
        {
            drawer.SetValues(this.mission);
        }
    }
}