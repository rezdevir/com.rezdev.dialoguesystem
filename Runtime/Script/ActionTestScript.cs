
using UnityEngine;

public class ActionTestScript : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        
        transform.localEulerAngles += Vector3.forward;
    }

    // Call Function with Arg
     public void TestFunction_1Arg(string message)
    {
        Debug.Log("Test Function_1Arg :" + message);
       
    }
    public void TestFunction_2Arg(string message,int number)
    {
        Debug.Log("Test Function_2Arg :" + message +" and the number:"+number);
     
    }
        public void  TestFunction()
    {
        Debug.Log("Test Function ");
    }
}
