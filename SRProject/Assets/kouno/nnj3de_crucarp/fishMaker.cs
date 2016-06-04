using UnityEngine;
using System.Collections;

public class fishMaker : MonoBehaviour {
    public GameObject fish;
    public int number = 15;
    public int levelSizeMax = 30;
    public int levelSizeMin = 1;

    private Vector3 pos;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < number; i++)
        {
            pos = new Vector3(0, Random.Range(levelSizeMin, levelSizeMax), 0);
            var item = Instantiate(fish, pos, new Quaternion(0, 0, 0, 0)) as GameObject;
            item.transform.SetParent(this.gameObject.transform.parent);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
