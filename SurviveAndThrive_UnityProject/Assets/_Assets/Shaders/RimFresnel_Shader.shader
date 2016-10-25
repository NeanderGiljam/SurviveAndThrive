// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,dith:2,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9386,x:32719,y:32712,varname:node_9386,prsc:2|diff-2148-OUT,lwrap-9096-OUT;n:type:ShaderForge.SFN_Fresnel,id:1863,x:32135,y:32828,varname:node_1863,prsc:2;n:type:ShaderForge.SFN_Blend,id:472,x:32474,y:32727,varname:node_472,prsc:2,blmd:10,clmp:True|SRC-2978-OUT,DST-1456-OUT;n:type:ShaderForge.SFN_LightColor,id:9191,x:32135,y:32967,varname:node_9191,prsc:2;n:type:ShaderForge.SFN_Blend,id:1456,x:32325,y:32967,varname:node_1456,prsc:2,blmd:12,clmp:True|SRC-1863-OUT,DST-9191-RGB;n:type:ShaderForge.SFN_Blend,id:9096,x:32523,y:32924,varname:node_9096,prsc:2,blmd:6,clmp:True|SRC-2978-OUT,DST-1456-OUT;n:type:ShaderForge.SFN_Tex2d,id:6995,x:32371,y:32518,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_6995,prsc:2,tex:b66bceaf0cc0ace4e9bdc92f14bba709,ntxv:0,isnm:False;n:type:ShaderForge.SFN_LightAttenuation,id:908,x:32068,y:32686,varname:node_908,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7736,x:32313,y:32775,varname:node_7736,prsc:2|A-1456-OUT,B-908-OUT;n:type:ShaderForge.SFN_Blend,id:2148,x:32682,y:32539,varname:node_2148,prsc:2,blmd:6,clmp:True|SRC-2978-OUT,DST-9135-OUT;n:type:ShaderForge.SFN_Multiply,id:9135,x:32573,y:32261,varname:node_9135,prsc:2|A-1520-OUT,B-472-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1520,x:32268,y:32297,ptovrint:False,ptlb:Rim_Strength,ptin:_Rim_Strength,varname:node_1520,prsc:2,glob:False,v1:0.33;n:type:ShaderForge.SFN_SwitchProperty,id:2978,x:32173,y:32436,ptovrint:False,ptlb:node_2978,ptin:_node_2978,varname:node_2978,prsc:2,on:True|A-6150-RGB,B-6995-RGB;n:type:ShaderForge.SFN_Color,id:6150,x:32031,y:32274,ptovrint:False,ptlb:node_6150,ptin:_node_6150,varname:node_6150,prsc:2,glob:False,c1:0.5,c2:0.5,c3:0.5,c4:1;proporder:6995-1520-6150-2978;pass:END;sub:END;*/

Shader "Custom/RimFresnel" {
    Properties {
        _Texture ("Texture", 2D) = "white" {}
        _Rim_Strength ("Rim_Strength", Float ) = 0.33
        _node_6150 ("node_6150", Color) = (0.5,0.5,0.5,1)
        [MaterialToggle] _node_2978 ("node_2978", Float ) = 0.3647059
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float _Rim_Strength;
            uniform fixed _node_2978;
            uniform float4 _node_6150;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
                #ifndef LIGHTMAP_OFF
                    float4 uvLM : TEXCOORD6;
                #else
                    float3 shLight : TEXCOORD6;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float3 _node_2978_var = lerp( _node_6150.rgb, _Texture_var.rgb, _node_2978 );
                float3 node_1456 = saturate(((1.0-max(0,dot(normalDirection, viewDirection))) > 0.5 ?  (1.0-(1.0-2.0*((1.0-max(0,dot(normalDirection, viewDirection)))-0.5))*(1.0-_LightColor0.rgb)) : (2.0*(1.0-max(0,dot(normalDirection, viewDirection)))*_LightColor0.rgb)) );
                float3 w = saturate((1.0-(1.0-_node_2978_var)*(1.0-node_1456)))*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = forwardLight * attenColor;
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * saturate((1.0-(1.0-_node_2978_var)*(1.0-(_Rim_Strength*saturate(( node_1456 > 0.5 ? (1.0-(1.0-2.0*(node_1456-0.5))*(1.0-_node_2978_var)) : (2.0*node_1456*_node_2978_var) ))))));
/// Final Color:
                float3 finalColor = diffuse;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float _Rim_Strength;
            uniform fixed _node_2978;
            uniform float4 _node_6150;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                #ifndef LIGHTMAP_OFF
                    float4 uvLM : TEXCOORD5;
                #else
                    float3 shLight : TEXCOORD5;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float3 _node_2978_var = lerp( _node_6150.rgb, _Texture_var.rgb, _node_2978 );
                float3 node_1456 = saturate(((1.0-max(0,dot(normalDirection, viewDirection))) > 0.5 ?  (1.0-(1.0-2.0*((1.0-max(0,dot(normalDirection, viewDirection)))-0.5))*(1.0-_LightColor0.rgb)) : (2.0*(1.0-max(0,dot(normalDirection, viewDirection)))*_LightColor0.rgb)) );
                float3 w = saturate((1.0-(1.0-_node_2978_var)*(1.0-node_1456)))*0.5; // Light wrapping
                float3 NdotLWrap = NdotL * ( 1.0 - w );
                float3 forwardLight = max(float3(0.0,0.0,0.0), NdotLWrap + w );
                float3 directDiffuse = forwardLight * attenColor;
                float3 diffuse = directDiffuse * saturate((1.0-(1.0-_node_2978_var)*(1.0-(_Rim_Strength*saturate(( node_1456 > 0.5 ? (1.0-(1.0-2.0*(node_1456-0.5))*(1.0-_node_2978_var)) : (2.0*node_1456*_node_2978_var) ))))));
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
