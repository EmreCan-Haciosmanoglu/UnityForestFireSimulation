using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestTree
{
    public GameObject burntIcon;
    public GameObject fire;
    public List<ForestTree> closestTrees = new List<ForestTree>();
    private bool Burnt = false;
    private bool OnFire = false;

    public float BurnTime;
    public float SpreadSpeed;
    public float SpreadRange = 0f;

    private ParticleSystem.ShapeModule shape;
    private ParticleSystem.EmissionModule emission;

    public ForestTree(GameObject burntIcon, GameObject fire, float burnTime, float spreadSpeed)
    {
        this.burntIcon = burntIcon;
        this.fire = fire;
        this.BurnTime = burnTime;
        this.SpreadSpeed = spreadSpeed;

        burntIcon.SetActive(false);
        fire.SetActive(false);

        ParticleSystem particleSystem = fire.GetComponent<ParticleSystem>();
        shape = particleSystem.shape;
        emission = particleSystem.emission;
    }
    

    private void SpreadFire()
    {
        foreach (ForestTree ctree in closestTrees)
        {
            if(!ctree.IsOnFire())
            {
                float dist = Vector3.Distance(burntIcon.transform.position, ctree.burntIcon.transform.position);
                if (dist < SpreadRange)
                    ctree.SetOnFire();
            }
        }
    }

    public bool IsBurnt()
    {
        return Burnt;
    }

    public void SetOnFire()
    {
        if (Burnt)
            return;
        Debug.Log("Fire!!");
        OnFire = true;
        fire.SetActive(true);
        MapGenerator.treesOnFire.Add(this);
    }

    public bool IsOnFire()
    {
        return OnFire;
    }

    public void Update()
    {
        if (!OnFire)
            return;
        float dt = Time.deltaTime;
        BurnTime -= dt;
        SpreadRange += dt * SpreadSpeed;

        shape.radius = (SpreadRange + 1f) / 10f;
        emission.rateOverTime = (SpreadRange + 1f) * 15;


        if (BurnTime < 0f)
        {
            Burnt = true;
            OnFire = false;
            fire.SetActive(false);
            burntIcon.SetActive(true);
            MapGenerator.treesOnFire.Remove(this);
            return;
        }
        SpreadFire();
    }
}
