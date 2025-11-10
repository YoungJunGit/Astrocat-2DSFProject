using DataEntity;
using DataEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrowdControlFactory
{
    public static ICrowdControl CreateCC(ELEMENT_STATUS_CATEGORY category)
    {
        switch (category)
        {
            case ELEMENT_STATUS_CATEGORY.STUN:
                return new Stun();
            case ELEMENT_STATUS_CATEGORY.BURN:
                return new Burn();
            case ELEMENT_STATUS_CATEGORY.CONTAMINATION:
                return new Contamination();
            case ELEMENT_STATUS_CATEGORY.SUPPRESS:
                return new Suppress();
            case ELEMENT_STATUS_CATEGORY.STRANGE:
                return new Strange();
            case ELEMENT_STATUS_CATEGORY.SILENCE:
                return new Silence();
            case ELEMENT_STATUS_CATEGORY.WEAKNESS:
                return new Weakness();
            case ELEMENT_STATUS_CATEGORY.OVERHEAT:
                return new Overheat();
            case ELEMENT_STATUS_CATEGORY.EXPOSURE:
                return new Exposure();
            case ELEMENT_STATUS_CATEGORY.BIND:
                return new Bind();
            case ELEMENT_STATUS_CATEGORY.CORRODE:
                return new Corrode();
            case ELEMENT_STATUS_CATEGORY.DOMINATE:
                return new Dominate();
            case ELEMENT_STATUS_CATEGORY.CHAOS:
                return new Chaos();
        }

        return null;
    }
}