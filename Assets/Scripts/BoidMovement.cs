using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidMovement : MonoBehaviour
{
    public GameObject[] boids;
    public Transform transform;    
    public Vector2 frame;
    public float boidSpeed;

    public Vector2 pos;
    public Vector2 vel;
    public float rot;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        transform.eulerAngles = new Vector3 (0, 0, rot - 45);
        boids = GameObject.FindGameObjectsWithTag("Boid");
    }

    void Bounce()
    {
        if (Mathf.Abs(pos.x) > frame.x/2)
        {
            vel.x *= -1;
            pos.x -= (Mathf.Abs(pos.x) - frame.x/2) * Mathf.Sign(pos.x);
        }
        if (Mathf.Abs(pos.y) > frame.y/2)
        {
            vel.y *= -1;
            pos.y -= (Mathf.Abs(pos.y) - frame.y/2) * Mathf.Sign(pos.y);
        }
    }

    private (float x, float y) Flock(float distance, float power)
    {
        int nCount = 0;
        float nSumX = 0;
        float nSumY = 0;
        foreach (GameObject otherb in boids)
        {
            if (Vector2.Distance(otherb.transform.position, pos) < distance)
            {
                nSumX += otherb.transform.position.x;
                nSumY += otherb.transform.position.y;
                nCount += 1;
            }
        }
        if (nCount > 0)
        {
            float deltaCenterX = nSumX/nCount - pos.x;
            float deltaCenterY = nSumY/nCount - pos.y;
            return (deltaCenterX * (power/1000), deltaCenterY * (power/1000)); 
        } else
        {
            return (0,0);
        }
        

        //var neighbors = boids.Where(otherb => Vector2.Distance(otherb.transform.position, pos) < distance);
        //float meanX = neighbors.Sum(otherb => otherb.transform.position.x) / neighbors.Count();
        //float meanY = neighbors.Sum(otherb =>otherb.transform.position.y) / neighbors.Count();
        //Vector2 deltaCenter = new Vector2(meanX - pos.x, meanY - pos.y);
        //return deltaCenter * power;
    }

    private (float x, float y) Align(float distance, float power)
    {
        int nCount = 0;
        float nSumX = 0;
        float nSumY = 0;
        foreach (GameObject otherb in boids)
        {
            if (Vector2.Distance(otherb.transform.position, pos) < distance)
            {
                //nSumX += otherb.BoidMovement.vel.x;
                //nSumY += otherb.BoidMovement.vel.y;
                nCount += 1;
            }
        }
        if (nCount > 0)
        {
            float deltaVelX = nSumX/nCount - vel.x;
            float deltaVelY = nSumY/nCount - vel.y;
            return (deltaVelX * (power/1000), deltaVelY * (power/1000)); 
        } else
        {
            return (0,0);
        }
    }


    private (float x, float y) Avoid(float distance, float power)
    {
        (float sumClosenessX, float sumClosenessY) = (0, 0);
        foreach (GameObject otherb in boids)
        {
            if (Vector2.Distance(otherb.transform.position, pos) < distance)
            {
                float closeness = distance - Vector2.Distance(otherb.transform.position, pos);
                sumClosenessX += (pos.x - otherb.transform.position.x) * closeness;
                sumClosenessY += (pos.y - otherb.transform.position.y) * closeness;
            }
        }
        return (sumClosenessX * power/1000, sumClosenessY * power/1000);
    }

    // Update is called once per frame
    void Update()
    {
        Bounce();
        //Debug.Log($"{vel} ,{Flock(1000, 2)} , {boids}");
        (float flockx, float flocky) = Flock(10, 3);
        (float avoidx, float avoidy) = Avoid(2, 30);
        vel.x += flockx + avoidx;
        vel.y += flocky + avoidy;
        pos += vel * Time.deltaTime;

        rot = Vector2.SignedAngle(Vector2.right, vel);
        transform.eulerAngles = new Vector3 (0, 0, rot - 45);
        transform.position = pos;
    }

    void OnGUI()
    {
        //GUI.Label(new Rect(25, 25, 200, 40), $"{rot}");
    }
}
