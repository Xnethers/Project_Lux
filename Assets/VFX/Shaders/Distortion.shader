// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:32716,y:32678,varname:node_4795,prsc:2|alpha-2142-OUT,refract-8247-OUT;n:type:ShaderForge.SFN_Tex2d,id:4644,x:31582,y:32585,ptovrint:False,ptlb:Refraction Map,ptin:_RefractionMap,varname:node_4644,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:5919,x:31582,y:32757,ptovrint:False,ptlb:Refraction Intensity,ptin:_RefractionIntensity,varname:node_5919,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5811966,max:1;n:type:ShaderForge.SFN_Tex2d,id:8878,x:31977,y:32886,ptovrint:False,ptlb:Opacity Map,ptin:_OpacityMap,varname:node_8878,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_ValueProperty,id:5099,x:32324,y:32854,ptovrint:False,ptlb:Opacity Value,ptin:_OpacityValue,varname:node_5099,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:2142,x:32499,y:32854,varname:node_2142,prsc:2|A-5099-OUT,B-8878-R;n:type:ShaderForge.SFN_Multiply,id:8207,x:32265,y:32948,varname:node_8207,prsc:2|A-9683-OUT,B-8878-R;n:type:ShaderForge.SFN_VertexColor,id:5031,x:32033,y:33085,varname:node_5031,prsc:2;n:type:ShaderForge.SFN_Color,id:6373,x:32033,y:33241,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_6373,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:1583,x:32265,y:33085,varname:node_1583,prsc:2|A-5031-A,B-6373-A;n:type:ShaderForge.SFN_Multiply,id:8247,x:32499,y:32990,varname:node_8247,prsc:2|A-8207-OUT,B-1583-OUT;n:type:ShaderForge.SFN_ComponentMask,id:9268,x:31802,y:32585,varname:node_9268,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-4644-RGB;n:type:ShaderForge.SFN_Multiply,id:9683,x:31977,y:32727,varname:node_9683,prsc:2|A-9268-OUT,B-5919-OUT;proporder:4644-5919-8878-5099-6373;pass:END;sub:END;*/

Shader "Shader Forge/Distortion" {
    Properties {
        _RefractionMap ("Refraction Map", 2D) = "white" {}
        _RefractionIntensity ("Refraction Intensity", Range(0, 1)) = 0.5811966
        _OpacityMap ("Opacity Map", 2D) = "white" {}
        _OpacityValue ("Opacity Value", Float ) = 0
        _Color ("Color", Color) = (1,1,1,1)
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _RefractionMap; uniform float4 _RefractionMap_ST;
            uniform float _RefractionIntensity;
            uniform sampler2D _OpacityMap; uniform float4 _OpacityMap_ST;
            uniform float _OpacityValue;
            uniform float4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                float4 projPos : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 _RefractionMap_var = tex2D(_RefractionMap,TRANSFORM_TEX(i.uv0, _RefractionMap));
                float4 _OpacityMap_var = tex2D(_OpacityMap,TRANSFORM_TEX(i.uv0, _OpacityMap));
                float2 sceneUVs = (i.projPos.xy / i.projPos.w) + (((_RefractionMap_var.rgb.rg*_RefractionIntensity)*_OpacityMap_var.r)*(i.vertexColor.a*_Color.a));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = 0;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,(_OpacityValue*_OpacityMap_var.r)),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
