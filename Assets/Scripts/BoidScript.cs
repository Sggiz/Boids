using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoidScript
{
    public class Boid

    {
        public Vector2 pos;
        public Vector3 pos3;
        public Vector2 vel;
        public float rot;
        public Vector3 rot3;
        public GameObject Obj;
        public Boid[] boidG;
        public Boid[] predG;
        public Vector2 frame;
        public Rigidbody rb;
        public Transform transform;
        public SpriteRenderer spriteR;

        public void InitSpriteR(Sprite spriteB, Material boidMat)
        {
            spriteR = Obj.AddComponent<SpriteRenderer>();
            spriteR.sprite = spriteB;
            spriteR.material = boidMat;
        }

        public void InitRb()
        {
            rb = Obj.AddComponent<Rigidbody>();
            rb.mass = 1;
            rb.useGravity = false;
            Vector2 InitPos = new Vector2 (Random.Range(-frame.x, frame.x), Random.Range(-frame.y, frame.y));
            pos = InitPos;
            float InitVelRange = 4f;
            Vector2 InitVel = new Vector2 (Random.Range(-InitVelRange,InitVelRange), Random.Range(-InitVelRange,InitVelRange));
            vel = InitVel;
        }

        public Boid(int n, Vector2 FrameDef, Sprite spriteB, Material boidMat)
        {
            Obj = new GameObject($"Boid{n}");
            frame = FrameDef;
            InitRb();
            InitSpriteR(spriteB, boidMat);
        }

        void Bounce(Vector2 BounceS)
        {
            if (Mathf.Abs(pos.x) > frame.x)
            {
                vel.x *= -1;
                pos.x -= (Mathf.Abs(pos.x) - frame.x) * Mathf.Sign(pos.x);
            }
            if (Mathf.Abs(pos.y) > frame.y)
            {
                vel.y *= -1;
                pos.y -= (Mathf.Abs(pos.y) - frame.y) * Mathf.Sign(pos.y);
            }
            
            float pad = BounceS.x;
            float turn = BounceS.y;
            if (Mathf.Abs(pos.x) > frame.x - pad)
                vel.x += turn * Mathf.Sign(pos.x);
            if (Mathf.Abs(pos.y) > frame.y - pad)
                vel.y += turn * Mathf.Sign(pos.y);
            
        }

        private Vector2 Flock(float distance, float power)
        {
            int nCount = 0;
            Vector2 deltaCenter = Vector2.zero;
            Vector2 nSum = Vector2.zero;
            foreach (Boid otherb in boidG)
            {
                if (Vector2.Distance(otherb.pos, pos) < distance)
                {
                    nSum += otherb.pos;
                    nCount += 1;
                }
            }
            if (nCount > 0)
                deltaCenter = nSum/nCount - pos;
            
            return deltaCenter * power;
        }

        private Vector2 Align(float distance, float power)
        {
            int nCount = 0;
            Vector2 nSum = Vector2.zero;
            Vector2 deltaVel = Vector2.zero;
            foreach (Boid otherb in boidG)
            {
                if (Vector2.Distance(otherb.pos, pos) < distance)
                {
                    nSum += otherb.vel;
                    nCount += 1;
                }
            }
            if (nCount > 0)
                deltaVel = nSum/nCount - vel;
            return deltaVel * power;
        }

        private Vector2 Avoid(float distance, float power)
        {
            Vector2 sumCloseness = Vector2.zero;
            foreach (Boid otherb in boidG)
            {
                float dist = Vector2.Distance(otherb.pos, pos);
                if (dist < distance)
                {
                    float closeness = distance - dist;
                    sumCloseness += (pos - otherb.pos) * closeness;
                }
            }
            return sumCloseness * power;
        }

        private Vector2 Predator(float distance, float power)
        {
            Vector2 sumCloseness = Vector2.zero;
            foreach (Boid p in predG)
            {
                float dist = Vector2.Distance(p.pos, pos);
                if (dist < distance)
                {
                    float closeness = distance - dist;
                    sumCloseness += (pos - p.pos) * closeness;
                }
            }
            return sumCloseness * power;
        }

        private void SLimit(Vector2 sl)
        {
            if (vel.magnitude < sl.x)
            {
                vel = vel.normalized * sl.x;
            }
            else if (vel.magnitude > sl.y)
            {
                vel = vel.normalized * sl.y;
            }
        }

        public void StartBoid(Boid[] bg, int pct, Material pm)
        {
            boidG = bg;
            predG = new Boid[pct];
            for (int i = 0; i < pct; i++)
            {
                predG[i] = boidG[i];
                if (Obj.name == ($"Boid{i}"))
                    spriteR.material = pm;
            }
            
        }

        public void UpdateBoid(Vector2 sl, Vector2 FlockS, Vector2 AlignS, Vector2 AvoidS, Vector2 PredS, Vector2 BounceS)
        {
            Bounce(BounceS);
            
            Vector2 flockVel = Flock(FlockS.x, FlockS.y);
            Vector2 alignVel = Align(AlignS.x, AlignS.y);
            Vector2 avoidVel = Avoid(AvoidS.x, AvoidS.y);
            Vector2 predVel = Predator(PredS.x, PredS.y);
            vel += flockVel + alignVel + avoidVel + predVel;
            SLimit(sl);
            
            pos += vel * Time.deltaTime;
            pos3 = pos;
            rb.position = pos3;
            rot = Vector2.SignedAngle(Vector2.right, vel);
            rot3.z = rot - 45;
            rb.rotation = Quaternion.Euler(rot3);
        }
    }
}