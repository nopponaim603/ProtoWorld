﻿using UnityEngine;
using System.Collections;
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.UI;

class DistinctLoggableComparer : IEqualityComparer<Loggable> {

	public bool Equals(Loggable x, Loggable y) {
		return ((MonoBehaviour)x).tag.Equals(((MonoBehaviour)y).tag);
	}

	public int GetHashCode(Loggable obj) {
		return ((MonoBehaviour)obj).tag.GetHashCode ();
	}
}

public class HistoricalDataController : MonoBehaviour
{
    public int logIntervalSeconds = 3;

    public DirectoryInfo logDirectory;
    private ScrollRect FileScrollView;
    private ScrollRect TimestampScrollView;

    public GameObject logFileButtonPrefab;
    public GameObject loadFileBrowser;

    private string logFileName;

    private XDocument logFile;

    private TimeController timeController;

    private FlashPedestriansGlobalParameters globalParameters;

    private CameraControl camera;

    private WaitForSeconds waitForLog;

	private bool exiting;


	//TODO: logfile reset after loading of snapshot

	void Start()
    {
        getLoggables();
        timeController = GameObject.Find("TimeControllerUI").GetComponent<TimeController>();
        GameObject flashPedestriansModule = GameObject.Find("FlashPedestriansModule");
        globalParameters = flashPedestriansModule.GetComponent<FlashPedestriansGlobalParameters>();
        initiateLogFile();
        createLogDestination();
        initiateLoggingInterface();

        waitForLog = new WaitForSeconds(logIntervalSeconds);

        StartCoroutine(logToXML());
    }

    //TODO: Optimize! THIS IS A HORSETHING!!!//
    List<Loggable> getLoggables()
    {
        List<GameObject> loggableObjects = new List<GameObject>();
        loggableObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)).Cast<GameObject>().Where(g => g.tag == "Loggable").ToList();
        List<Loggable> loggables = new List<Loggable>();
        foreach (GameObject loggable in loggableObjects)
        {
            print(loggable.ToString());
            loggables.Add(loggable.GetComponent<Loggable>());
        }
        //print(loggables.Count);
        return loggables;
    }

    void initiateLogFile()
    {
        //Create XML document for logging data with root element "LogData" 
        logFile = new XDocument();
        logFile.Add(new XElement("LogData"));
    }

    void createLogDestination()
    {
        //Define path to the log folder and create it if it doesn't exist
        logDirectory = new DirectoryInfo(Application.dataPath + "/log/");
        if (!logDirectory.Exists) logDirectory.Create();
    }

    void initiateLoggingInterface()
    {
        //FileScrollView = GameObject.Find("FileScrollView").GetComponent<ScrollRect>();
        loadFileBrowser = GameObject.Find("LoadFileBrowser");
        loadFileBrowser.SetActive(false);
        //TimestampScrollView = GameObject.Find("TimestampScrollView").GetComponent<ScrollRect>();
        camera = GameObject.Find("Main Camera").GetComponent<CameraControl>();
    }

    public IEnumerator logToXML()
    {
        //As long as the simulation is not paused log the data of each subscribed loggable object
        while (!globalParameters.flashPedestriansPaused && !exiting)
        {
			XElement timeStamp = new XElement("TimeStamp");
			timeStamp.Add(new XAttribute("timestamp", timeController.gameTime));
			logFile.Root.Add(timeStamp);
			foreach (Loggable loggable in LoggableManager.getCurrentSubscribedLoggables())
			{
				timeStamp.Add(convertDataToXML(loggable.getLogData()));
			}
            yield return waitForLog;
        }
    }

    //Recursive call for logging a tree to XML
    XElement convertDataToXML(LogDataTree logData)
    {
        XElement element = new XElement(logData.Key, logData.Value);
        foreach (LogDataTree child in logData.getChildren())
        {
            element.Add(convertDataToXML(child));
        }
        return element;
    }

    public void SaveHistoricalData()
    {
        KPIParameters.pauseKPIS = true;
        timeController.PauseGame(true);
        logFileName = DateTime.Now.ToString("ddMMyyyy_HH-mm-ss");
        logFile.Save(logDirectory + "/" + logFileName + ".xml");
        timeController.PauseGame(false);
    }

    public void loadLogDirectory()
    {
        FileBrowserController controller = loadFileBrowser.GetComponent<FileBrowserController>();
        controller.loadLogDirectory();
        loadFileBrowser.SetActive(true);
        timeController.PauseGame(true);
        KPIParameters.pauseKPIS = false;
    }

    public void recreateLog(string file, string timestamp)
    {
        //print((from element in XDocument.Load(file).Root.Descendants("TimeStamp") where element.FirstAttribute.Value.Equals(timestamp) select element).FirstOrDefault());
        XElement timeStampElement =
            (from element in XDocument.Load(file).Root.Descendants("TimeStamp")
             where element.FirstAttribute.Value.Equals(timestamp)
             select element).FirstOrDefault();

        // TODO Use the interface to allow objects to destroy (or recycle, etc.) themselves.
        removeActiveData();
        recreateObjects(timeStampElement);

        loadFileBrowser.SetActive(false);
        timeController.PauseGame(false);
    }

    //Recreates the logged loggables in order of priority
    private void recreateObjects(XElement timestampElement)
    {
        //Get all objects which have the loggable interface with a unique tag
        List<Loggable> loggables = InterfaceHelper.FindObjectsInResources<Loggable>().Distinct(new DistinctLoggableComparer()).ToList();
        //List<Loggable> loggables = getLoggables();
        foreach (LogPriorities priority in Enum.GetValues(typeof(LogPriorities)))
        {
            foreach (Loggable loggable in loggables.FindAll(item => item.getPriorityLevel() == priority))
            {
                foreach (XElement loggedObject in timestampElement.Descendants(((MonoBehaviour)loggable).tag))
                {
                    // TODO Instead of recreating spawners and destinations we can just update the properties that
                    // change over time. This temporary if statement shows that it works without recreating them.
                    if (!(loggable is FlashPedestriansSpawner) && !(loggable is FlashPedestriansDestination))
                    {
                        loggable.rebuildFromLog(rebuildObjectLogData(loggedObject));
                    }
                }
            }
        }
    }

    //Recreates the logged loggables in order of priority (using bins)
    private void recreateObjectsBins(XElement timestampElement)
    {
        //Get all objects which have the loggable interface with a unique tag
        List<Loggable> loggables = InterfaceHelper.FindObjectsInResources<Loggable>().Distinct(new DistinctLoggableComparer()).ToList();

        Array priorities = Enum.GetValues(typeof(LogPriorities));
        Array.Reverse(priorities);
        //crititcal prio
        //high prio
        //medium prio

        //create lists
        List<List<Loggable>> prioLoggables = new List<List<Loggable>>();
        foreach (LogPriorities priority in priorities)
        {
            prioLoggables.Add(new List<Loggable>());
        }

        //put each loggable into the correct bin
        foreach (Loggable loggable in loggables)
        {
            int index = Array.IndexOf(priorities, loggable.getPriorityLevel());
            prioLoggables[index].Add(loggable);
        }

        //for each priority handle the loggables
        foreach (List<Loggable> prioLogs in prioLoggables)
        {
            foreach (Loggable loggable in prioLogs)
            {
                foreach (XElement loggedObject in timestampElement.Descendants(((MonoBehaviour)loggable).tag))
                {
                    if (!(loggable is FlashPedestriansSpawner) && !(loggable is FlashPedestriansDestination))
                    {
                        loggable.rebuildFromLog(rebuildObjectLogData(loggedObject));
                    }
                }
            }
        }
    }

    //Recursively transforms the XML data of a loggable to a LogDataTree
    private LogDataTree rebuildObjectLogData(XElement element)
    {
        LogDataTree logData = new LogDataTree(element.Name.ToString(), element.Value);
        foreach (XElement child in element.Descendants())
        {
            logData.AddChild(rebuildObjectLogData(child));
        }
        return logData;
    }

    private void removeActiveData()
    {
        FlashPedestriansSpawner.nextIdForPedestrian = 0;
        FlashPedestriansInformer flashInformer = FindObjectOfType<FlashPedestriansInformer>();
        flashInformer.accumPedestrians = 0;
        flashInformer.activePedestrians = new Dictionary<int, FlashPedestriansController>();
        foreach (Loggable loggable in LoggableManager.getCurrentSubscribedLoggables())
        {
            // TODO Instead of destroying spawners and destinations we can just update the properties that
            // change over time. This temporary if statement shows that it works without destroying them.
            if (loggable.destroyOnLogLoad())
            {
                LoggableManager.unsubscribe(loggable);
                Destroy(((MonoBehaviour)loggable).gameObject);
            }
        }
    }


	/// <summary>
	/// Prevents logging while the application is closing.
	/// </summary>
	private void OnApplicationQuit() {
		exiting = true;
	}
}
