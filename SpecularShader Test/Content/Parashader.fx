#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif


float4x4 WorldViewProjection;

float4 AmbientColor = float4(1, 1, 1, 1);
float AmbientIntensity = 0.4;




float4 DiffuseColor = float4(1, 1, 1, 1);
float DiffuseIntensity = 1.0;

float4 Reflection = float4(0,0,1,0);

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Normal : NORMAL0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	
	
	output.Position = mul(input.Position, WorldViewProjection);
	output.Position = output.Position / output.Position.w; 
	
	float L = length(output.Position.xyz);
    output.Position = output.Position/L;  
	
	output.Position.z = output.Position.z + 1.0; 
	output.Position.x = output.Position.x / output.Position.z;
	output.Position.y = output.Position.y / output.Position.z;

	output.Position.z = (L -0.1)/(100-0.1);
	output.Position.w = 1; 
		
	
	output.Color = AmbientColor * AmbientIntensity;


	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
	return input.Color; 

	
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};