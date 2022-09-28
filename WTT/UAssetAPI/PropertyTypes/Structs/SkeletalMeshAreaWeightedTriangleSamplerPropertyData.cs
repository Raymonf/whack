﻿using UAssetAPI.UnrealTypes;

namespace UAssetAPI.PropertyTypes.Structs
{
    public class SkeletalMeshAreaWeightedTriangleSamplerPropertyData : WeightedRandomSamplerPropertyData
    {
        public SkeletalMeshAreaWeightedTriangleSamplerPropertyData(FName name) : base(name)
        {

        }

        public SkeletalMeshAreaWeightedTriangleSamplerPropertyData()
        {

        }

        private static readonly FString CurrentPropertyType = new FString("SkeletalMeshAreaWeightedTriangleSampler");
        public override bool HasCustomStructSerialization { get { return true; } }
        public override FString PropertyType { get { return CurrentPropertyType; } }
    }
}