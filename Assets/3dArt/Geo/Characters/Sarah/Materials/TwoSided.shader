Shader "Standard/TwoSided"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _EmissionColor ("Emission Color", Color) = (0,0,0)
        _EmissionMap ("Emission", 2D) = "black" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Cull Off // Disables backface culling for two-sided rendering

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_EmissionMap;
        };

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _EmissionMap;

        half _Metallic;
        half _Smoothness;
        fixed4 _Color;
        fixed4 _EmissionColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 albedoColor = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = albedoColor.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            o.Emission = tex2D(_EmissionMap, IN.uv_EmissionMap) * _EmissionColor;
            o.Alpha = albedoColor.a;
        }
        ENDCG
    }
    FallBack "Standard"
}