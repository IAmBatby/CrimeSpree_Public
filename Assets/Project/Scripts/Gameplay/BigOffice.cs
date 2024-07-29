using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigOffice : MonoBehaviour
{
    public enum BigOfficeType { Office, IT}
    public BigOfficeType bigOfficeType;
    public Animator bigOfficeDoor;
    [ShowIf("bigOfficeType", BigOfficeType.Office)] public GameObject colorObject;
    [ShowIf("bigOfficeType", BigOfficeType.Office)] public GameObject colorBanner;
    [ShowIf("bigOfficeType", BigOfficeType.Office)] public List<NPC> NPCs;
    [ShowIf("bigOfficeType", BigOfficeType.Office)] public WaypointCollection officeWaypointCollection;
    [ShowIf("bigOfficeType", BigOfficeType.IT)] public SecurityComputer securityComputer;
    [ShowIf("bigOfficeType", BigOfficeType.IT)] public TriggerForwarder findITTrigger;
    [ShowIf("bigOfficeType", BigOfficeType.IT)] public TriggerForwarder enterITTrigger;
    [ShowIf("bigOfficeType", BigOfficeType.IT)] public NPC NPC_IT;
}
