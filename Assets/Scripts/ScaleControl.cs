using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleControl : MonoBehaviour {

    [SerializeField]
    GameObject bigCube;
    float a = 0.11f;

    List<GameObject> ScaledPoints;

	// Use this for initialization
	void Start () {
        //a = GetComponentInParent<Transform>().position.x - transform.position.x;
        Debug.Log(a);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateScale()
    {
        Debug.Log("UpdateScale");
        if (ScaledPoints != null)
        {
            foreach (GameObject g in ScaledPoints)
            {
                Destroy(g);
            }
            ScaledPoints.Clear();
        }
        else
        {
            ScaledPoints = new List<GameObject>();
        }

        List<GameObject> points = TrajectoryData.Instance.GetTrajectory();

        foreach (GameObject p in points)
        {
            Vector3 v = p.transform.position - transform.position;
            if (v.x < a && v.y < a && v.z < a)
            {
                GameObject p2 = Instantiate(p, bigCube.transform);
                p2.transform.position = bigCube.transform.position + v * 2;
                p2.transform.localScale = p2.transform.localScale * 2 / bigCube.transform.localScale.magnitude;
                p2.AddComponent<CopyControl>();

                p2.GetComponent<CopyControl>().origin = p;
                ScaledPoints.Add(p2);
            }
        }
        
    }
}
