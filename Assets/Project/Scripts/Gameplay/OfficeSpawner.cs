using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeSpawner : MonoBehaviour
{
    public OfficeSpawnerManager officeManager;
    public Randomiser officeRandomiser;
    public Randomiser colorRandomiser;
    public Animator BigOfficeDoor;
    public GameObject parent;


    void Start()
    {
        GameObject office = officeRandomiser.Randomise();
        GameObject officeObject = Instantiate(office, transform.position, office.transform.rotation);
        Debug.Log("Created" + officeObject);
        //officeObject.transform.SetParent(transform);
        officeObject.transform.position = transform.position;
        officeObject.transform.rotation = parent.transform.rotation;
        //GameManager.Instance.BatchObjects(officeObject);

        if (officeObject.TryGetComponent(out BigOffice bigOffice))
        {
            if (bigOffice.bigOfficeType == BigOffice.BigOfficeType.Office)
            {
                bigOffice.colorObject = colorRandomiser.Randomise();
                bigOffice.colorBanner.GetComponent<MeshRenderer>().material = bigOffice.colorObject.GetComponent<RandomColour>().color;
                if (bigOffice.colorObject.GetComponent<RandomColour>().name == "Red")
                    officeManager.bigOfficeRed = bigOffice;
                else if (bigOffice.colorObject.GetComponent<RandomColour>().name == "Blue")
                    officeManager.bigOfficeBlue = bigOffice;
                else if (bigOffice.colorObject.GetComponent<RandomColour>().name == "Yellow")
                    officeManager.bigOfficeYellow = bigOffice;
                else if (bigOffice.colorObject.GetComponent<RandomColour>().name == "Green")
                    officeManager.bigOfficeGreen = bigOffice;
                bigOffice.bigOfficeDoor = BigOfficeDoor;
                officeManager.officeList.Add(bigOffice);
                officeManager.TryFill();
            }
            else if (bigOffice.bigOfficeType == BigOffice.BigOfficeType.IT)
            {
                officeManager.officeIT = bigOffice;
                officeManager.TryFill();
            }
        }
    }
}
