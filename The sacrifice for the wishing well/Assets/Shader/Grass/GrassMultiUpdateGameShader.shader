Shader "CustomRenderTexture/GrassMultiUpdateGameShader"
{
	Properties
	{
		[HideInInspector]
		_CamData("Camera Data", Vector) = (0, 0, 0, 0)
		//[HideInInspector]
		//_ObjData("Object Data", Vector) = (0, 0, 0, 0)
		//[HideInInspector]
		//_ObjScale("Object Scale", Vector) = (0, 0, 0, 0)
		_ObjLength("Object Length", Int) = 0


		_Stiffness("Stiffness", Float) = .5
		_Damping("Damping", Float) = .75
		_SpeedDown("SpeedDown", Float) = 100

		[HideInInspector]
		_Scale("Scale", Float) = 10

		_Wind("Wind", Vector) = (0,0,1,5)
		_WindOff("Wind offset", Float) = .5

		_RotSpeed("RotationSpeed", Float) = 0

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

		   float _RotSpeed;

		   //Transform:
		   float _Scale;
		   float4 _CamData;
		   //ObjData besteht aus jeweils: xy: worldPosition, zw: velocity
		   uniform float4 _ObjData[100]; //<- muss via script initiiert und kleiner oder gleich diesem Wert gesetzt werden!
		   //ObjScale besteht aus jeweils: xy: Länge/Breite des Rechtecks, z: Eck-Radius, w: Verschleierung
		   uniform float4 _ObjScale[100];//<- muss via script initiiert und kleiner oder gleich diesem Wert gesetzt werden!
		   //ObjStrength besteht aus jeweils: x: push-faktor, y: abstoß-faktor, z: rotations-faktor
		   uniform float3 _ObjStrength[100];//<- muss via script initiiert und kleiner oder gleich diesem Wert gesetzt werden!
		   uniform int _ObjLength;

		   sampler2D _Tex;

		   float4 frag(v2f_customrendertexture IN) : COLOR
		   {
			   //Schritt 1:
			   //Berechne alle Änderungen durch Objekte:

			   float2 displ_nxt = float2(0, 0);
			   float2 vel_nxt = float2(0, 0);
			   float aspect = _CustomRenderTextureWidth / _CustomRenderTextureHeight;
			   float2 orthScale = _Scale/unity_OrthoParams.y * unity_OrthoParams * 2;

			   //Laufe jedes angegebene objekt ab:
			   for (int i = 0; i < _ObjLength; i++) {
				   float2 posRelativ = (_ObjData[i].xy - _WorldSpaceCameraPos) / orthScale + float2(.5, .5);//in Bereich: 0-1


				   //Wandle _ObjScale von Weltkoord. zu lokale Koord.(0-1) um:
				   float2 localScale = _ObjScale[i].xy / orthScale;

				   //Position:
				   float2 delta = posRelativ - IN.globalTexcoord.xy;//anstatt IN.globalTexcoord
				   //rounded Rectangle:
				   float x_pos = (max(abs(delta.x), localScale.x) - localScale.x) * aspect;
				   float y_pos = max(abs(delta.y), localScale.y) - localScale.y;

				   //Veschliere das Rechteck, damit das Gras schwächer zurück schnappt:
				   float angleDiff = abs(atan2(delta.y, delta.x * aspect) - atan2(_ObjData[i].w, _ObjData[i].z));
				   //angleDiff = min(min(angleDiff, 6.2831853 - angleDiff), 1.5707963 / 1.5) * 1.5;
				   float radius = _ObjScale[i].z * (1 + max(_ObjScale[i].w * cos(angleDiff), 0));

				   //Distance:
				   float distance = 1 - min(length(float2(x_pos, y_pos)) / max(radius, 0.00001) * orthScale.y, 1);//_ObjScale[i].z = Radialscale


				   //Verschiebe-Faktoren:
				   float2 delta_normal = normalize(delta);
				   float2 push_part = _ObjData[i].zw * _ObjStrength[i].x;
				   float2 attract_part = delta * _ObjStrength[i].y;
				   float2 rot_part = float2(delta_normal.y, -delta_normal.x) * _ObjStrength[i].z;


				   vel_nxt = vel_nxt + distance * (push_part + attract_part + rot_part); //Zusammenführung durch Überlagerung
			   }


			   //Scritt 2:
			   //Berechne displacement und velocity durch überlagerung aller effekte:

			   //hole daten von der rendertex:
			   float2 texCoord_real = IN.globalTexcoord.xy + _CamData.xy / orthScale;

			   float4 prev = tex2D(_SelfTexture2D, texCoord_real);
			   float2 displ_prev = prev.xy;
			   float2 vel_prev = prev.zw;
			   
			   //Displacement:
			   float2 displ_diff = (displ_prev - float2(.5, .5)) * _Stiffness;
			   displ_nxt = clamp(
				   displ_prev //gespeicherte verschiebung
				   - (displ_diff + vel_prev - float2(.5, .5)) / _SpeedDown//Änderung der Verschiebung
				   , 0, 1);

			   //Wind:
			   float Deg2Rad = .0174533;//PI/180
			   float2 windDir = float2(cos(_Wind.y * Deg2Rad), sin(_Wind.y * Deg2Rad));

			   //Rotation-displ:
			   float2 rot_vec = normalize(IN.globalTexcoord.xy - float2(.5, .5));
			   rot_vec = float2(rot_vec.y, -rot_vec.x);

			   //Geschwindigkeit:
			   vel_nxt = clamp(vel_nxt
				   + vel_prev //gespeicherte geschw.
				   + displ_diff * (1 - _Damping) //gedämpfte hinzugefügte geschw.
				   + _Wind.x * windDir * (Unity_SimpleNoise_float(texCoord_real * float2(aspect, 1) - _WorldSpaceCameraPos / orthScale -  _Time.y * windDir * _Wind.z, _Wind.w) - _WindOff)//wind
				   + rot_vec * _RotSpeed//rotation
				   , 0.1f, .9f);

			   return float4(displ_nxt, vel_nxt);
		   }
		   ENDCG
	   }
	}
}
