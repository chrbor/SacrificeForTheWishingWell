Shader "CustomRenderTexture/GrassMultiUpdateShader"
{
	Properties
	{
		[HideInInspector]
		_CamScale("Camera Scale", Vector) = (0, 0, 0, 0)
		//[HideInInspector]
		//_ObjData("Object Data", Vector) = (0, 0, 0, 0)
		//[HideInInspector]
		//_ObjScale("Object Scale", Vector) = (0, 0, 0, 0)
		_ObjLength("Object Length", Int) = 0


		_Stiffness("Stiffness", Float) = .5
		_Damping("Damping", Float) = .75
		_SpeedDown("SpeedDown", Float) = 100

		[HideInInspector]
		_Scale("Scale", Vector) = (.2,.2,0,0)

		_Wind("Wind", Vector) = (0,0,1,5)
		_WindOff("Wind offset", Float) = .5

		[HideInInspector]
		_Tex("InputTex", 2D) = "white" {}
	}

		SubShader
	{
	   Lighting Off
	   Blend One Zero

	   Pass
	   {
		   CGPROGRAM
		   #include "UnityCustomRenderTexture.cginc"
		   #include "NodeFcts.cginc"
		   #pragma vertex CustomRenderTextureVertexShader
		   #pragma fragment frag
		   #pragma target 3.0

		   //Constants:
		   float _Deg2Rad;

		   //Properties:
		   float _Stiffness;
		   float _Damping;
		   float _SpeedDown;
		  
		   //Wind:
		   float4 _Wind;
		   float _WindOff;

		   //Transform:
		   float4 _CamScale;
		   //ObjData besteht aus jeweils: xy: worldPosition, zw: velocity
		   float4 _ObjData[100]; //<- muss via script initiiert und kleiner oder gleich diesem Wert gesetzt werden!
		   //ObjScale besteht aus jeweils: xy: Länge/Breite des Rechtecks, z: Eck-Radius, w: Stärke-faktor
		   float4 _ObjScale[100];//<- muss via script initiiert und kleiner oder gleich diesem Wert gesetzt werden!
		   int _ObjLength;

		   sampler2D _Tex;

		   float4 frag(v2f_customrendertexture IN) : COLOR
		   {
			   //Schritt 1:
			   //Berechne alle Änderungen durch Objekte:

			   float2 displ_nxt = float2(0, 0);
			   float2 vel_nxt = float2(0, 0);
			   float aspect = _CustomRenderTextureWidth / _CustomRenderTextureHeight;


			   //Laufe jedes angegebene objekt ab:
			   for (int i = 0; i < _ObjLength; i++) {
				   float2 posRelativ = (_ObjData[i].xy - _WorldSpaceCameraPos) / _CamScale.xy + float2(.5, .5);//in Bereich: 0-1


				   //Wandle _ObjScale von Weltkoord. zu lokale Koord.(0-1) um:
				   float2 localScale = _ObjScale[i].xy / _CamScale.xy;// *float2(1.778, 1);//kA warum * float(aspect,1)...

				   //Position:
				   float2 delta = IN.globalTexcoord.xy - posRelativ;//anstatt IN.globalTexcoord
				   //rounded Rectangle:
				   float x_pos = (max(abs(delta.x), localScale.x) - localScale.x) * aspect;
				   float y_pos = max(abs(delta.y), localScale.y) - localScale.y;
				   //Distance:
				   float distance = 1 - min(length(float2(x_pos, y_pos)) * 2 / _ObjScale[i].z * _CamScale.y, 1);//Radialscale = .2
				   //Effect:
				   float effect = distance * _ObjScale[i].w;


				   vel_nxt = vel_nxt + effect * _ObjData[i].zw; //Berührung
			   }


			   //Scritt 2:
			   //Berechne displacement und velocity durch überlagerung aller effekte:

			   //hole daten von der rendertex:
			   float4 prev = tex2D(_SelfTexture2D, IN.globalTexcoord.xy);
			   float2 displ_prev = prev.xy;
			   float2 vel_prev = prev.zw;
			   
			   //Displacement:
			   float2 displ_diff = (displ_prev - float2(.5, .5)) * _Stiffness;
			   displ_nxt = clamp(displ_nxt
				   + displ_prev //gespeicherte verschiebung
				   - (displ_diff + vel_prev - float2(.5, .5)) / _SpeedDown//Änderung der Verschiebung
				   , 0, 1);

			   //Wind:
			   float Deg2Rad = .0174533;//PI/180
			   float2 windDir = float2(cos(_Wind.y * Deg2Rad), sin(_Wind.y * Deg2Rad));

			   //Geschwindigkeit:
			   vel_nxt = clamp(vel_nxt
				   + vel_prev //gespeicherte geschw.
				   + displ_diff * (1 - _Damping) //gedämpfte hinzugefügte geschw.
				   + _Wind.x * windDir * (Unity_SimpleNoise_float(IN.globalTexcoord.xy - _WorldSpaceCameraPos / _CamScale.xy +  _Time.y * windDir * _Wind.z, _Wind.w) - _WindOff)//wind
				   , 0, 1);

			   return float4(displ_nxt, vel_nxt);
		   }
		   ENDCG
	   }
	}
}
