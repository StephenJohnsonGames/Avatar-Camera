using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;

public class NR_GrabOutputs : MonoBehaviour
{

    public GameObject FPSCamera;
    public NaturalRenderingManager NRM;
    public int LocationCount;
    public int ListCount;
    public string ProjValue;
    public GameObject AvatarWoman;
    public GameObject NRStandardLook;
    public bool mouseData = false;
    public Toggle mouseD;
    public float Timer = 1.0f;
    private List<string> DataOutput = new List<string>();
    private List<string> ProjectionOutput = new List<string>();
    private List<string> TimeOutput = new List<string>();
    // Use this for initialization
    public Text currentOutput;
    Text currentOutString;
    void Start()
    {
        currentOutString = currentOutput.GetComponent<Text>();

        DataOutput.Clear();
        ProjectionOutput.Clear();
        ListCount = 1;
        LocationCount = 1;
        NRM = FPSCamera.GetComponent<NaturalRenderingManager>();
        mouseD.isOn = false;
        TimeOutput.Add("LookingUporDown" + "\t" + "CompassHeading" + "\t" + "Projection".ToString());

        ProjectionOutput.Add("DataSet" + "\t" + "AvatarPosX" + "\t" + "AvatarPosY" + "\t" + "AvatarPosZ" + "\t" + "MouseRotationX" + "\t" + "MouseRotationY" + "\t" + "Projection".ToString());
    }

    public enum NR_Style
    {
        NR_LINEAR,
        NR_FISHEYE_ORTHOGRAPHIC,
        NR_FISHEYE_EQUIDISTANT,
        NR_FISHEYE_STEREOGRAPHIC,
        NR_EQUIRECTANGULAR,
        NR_PANINI,
        NR_NATURAL_VULKAN,
        NR_BLENDING,
        LP_92,
        LP_113,
        LP_120,
        LP_163,
        NR_120,
        NR_142,
        NR_163

    }


    public NR_Style CurrentNRStyle
    {
        get;
        set;
    }

    // Update is called once per frame
    void Update()
    {
        currentOutString.text = NRM.CurrentNRStyle.ToString();
        //Debug.Log(NRM.CurrentNRStyle.ToString());


        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Return))
        {
            ProcessFOVData();

        }

        if (Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Return))
        {
            //TimeOutput.Add("MouseRotationX" + "\t" + "MouseRotationY" + "\t" + "Projection".ToString());
            //StartCoroutine(FixedUpdateData());
            mouseData = !mouseData;
            mouseD.isOn = mouseData;
            //if(mouseData == true)
            //{
            //    ListCount = ListCount + 1;
            //}

            InvokeRepeating("FixedUpdateData", 0.5f, 0.5f);

        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            //WriteKeyPressData();
            
            StartCoroutine(CaptureProcess());
        }

        if(mouseData == true)
        {

            //StartCoroutine(FixedUpdateData());
            //yield return new WaitForSeconds(Timer);
            //StopCoroutine(FixedUpdateData());
            

        }

        if(mouseData == false)
        {
            CancelInvoke();

            //StopCoroutine(FixedUpdateData());

        }
    }

    IEnumerator CaptureProcess()
    {

        NRM.CaptureMe = true;

        //output a series of images and data as desired for each location set by LocationCount
        float Pause = 0.5f;

        NRM.NR_SetLinear();
        ProjValue = "LP_92";
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/../Outputs/" + LocationCount.ToString("000") + "Linear_92H.png");
        yield return new WaitForSeconds(Pause);

        NRM.NR_SetLinear();
        ProjValue = NRM.CurrentNRStyle.ToString();
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/../Outputs/" + LocationCount.ToString("000") + "Linear_113H.png");
        yield return new WaitForSeconds(Pause);

        NRM.NR_SetLinear();
        NRM.NR_LP_FOV = 0.57f;
        ProjValue = NRM.CurrentNRStyle.ToString();
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/../Outputs/" + LocationCount.ToString("000") + "Linear_120H.png");
        yield return new WaitForSeconds(Pause);

        NRM.NR_SetLinear();
        NRM.NR_LP_FOV = 0.146f;
        ProjValue = NRM.CurrentNRStyle.ToString();
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/../Outputs/" + LocationCount.ToString("000") + "Linear_163H.png");
        yield return new WaitForSeconds(Pause);


        WriteKeyPressData();

        LocationCount = LocationCount + 1;

        NRM.CaptureMe = false;


    }



    public void WriteKeyPressData()
    {



        //DataOutput.Add ("ProjBlend" + "\t" + Camera1.GetComponent<PositionfromCameras> ()._Blend1.ToString ());
       
        DataOutput.Add("LocationCount" + "\t" + LocationCount.ToString());
        DataOutput.Add("Location" + "\t" + FPSCamera.transform.position.ToString());


        DataOutput.Add("Projection" + "\t" + NRM.CurrentNRStyle.ToString());

       

        TextWriter ExportData = new StreamWriter(Application.dataPath + "/../Outputs/" + LocationCount.ToString() + ".txt");

        for (int i = 0; i < DataOutput.Count; i++)
        {
            ExportData.WriteLine(DataOutput[i]);
        }
        ExportData.Close();





    }

    public void ProcessFOVData()
    {
        //if(LocationCount < 2)
        //{
        //    ProjectionOutput.Add("DataSet" + "\t" + "AvatarPosX" + "\t" + "AvatarPosY" + "\t" + "AvatarPosZ" + "\t" + "MouseRotationX" + "\t" + "MouseRotationY" + "\t" + "Projection".ToString());
        //}

        //ProjectionOutput.Add("DataSet" + "\t" + "AvatarPosX" + "\t" + "AvatarPosY" + "\t" + "AvatarPosZ" + "\t" + "MouseRotationX" + "\t" + "MouseRotationY" + "\t" + "Projection".ToString());
        ProjectionOutput.Add(LocationCount + "\t" + AvatarWoman.transform.position.x + "\t" + AvatarWoman.transform.position.y + "\t" + AvatarWoman.transform.position.z + "\t" + NRStandardLook.transform.eulerAngles.x + "\t" + NRStandardLook.transform.eulerAngles.y + "\t" + NRM.CurrentNRStyle.ToString());

        //ProjectionOutput.Add("DataSet" + "\t" + LocationCount.ToString());
        //ProjectionOutput.Add("AvatarPosX" + "\t" + AvatarWoman.transform.position.x.ToString());
        //ProjectionOutput.Add("AvatarPosY" + "\t" + AvatarWoman.transform.position.y.ToString());
        //ProjectionOutput.Add("AvatarPosZ" + "\t" + AvatarWoman.transform.position.z.ToString());
        //ProjectionOutput.Add("MouseRotationX" + "\t" + NRStandardLook.transform.eulerAngles.x.ToString());
        //ProjectionOutput.Add("MouseRotationY" + "\t" + NRStandardLook.transform.eulerAngles.y.ToString());
        //ProjectionOutput.Add("Projection" + "\t" + NRM.CurrentNRStyle.ToString());
        LocationCount = LocationCount + 1;

        TextWriter ExportData = new StreamWriter(Application.dataPath + "/../Outputs/" + "PositionTask".ToString() + ".txt");

        for (int i = 0; i < ProjectionOutput.Count; i++)
        {
            ExportData.WriteLine(ProjectionOutput[i]);
        }
        ExportData.Close();
    
    }

    public void FixedUpdateData()
    {
        //ProjectionOutput.Add("DataSet" + "\t" + LocationCount.ToString());
        //ProjectionOutput.Add("Time" + "\t" + Time.time.ToString());
        //TimeOutput.Add("MouseRotationX" + "\t" + "MouseRotationY" + "\t" + "Projection".ToString());

        //TimeOutput.Add("MouseRotationY" + "\t" + NRStandardLook.transform.eulerAngles.y.ToString());
        //TimeOutput.Add("Projection" + "\t" + NRM.CurrentNRStyle.ToString());

        TimeOutput.Add(NRM.OutputAngleUporDown + "\t" + NRM.NorthSouthEastWest + "\t" + NRM.CurrentNRStyle.ToString());

        
        TextWriter ExportData = new StreamWriter(Application.dataPath + "/../Outputs/" + "RotationDuringNavigationTask".ToString() + ".txt");

        for (int i = 0; i < TimeOutput.Count; i++)
        {
            ExportData.WriteLine(TimeOutput[i]);
        }
        ExportData.Close();


    }

}

