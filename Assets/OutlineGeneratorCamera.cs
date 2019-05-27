using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[ExecuteInEditMode]
public class OutlineGeneratorCamera : SyncronizedCamera
{
    [SerializeField] private int screenDivide = 2;
    [SerializeField] private Material procMaterial;
    [SerializeField] private Material compMaterial;


    private CommandBuffer cmdAfterGenerator = null;
    private CommandBuffer cmdAfterMain = null;
    private RenderTexture depthExRTEntity;
    private RenderTexture infratedDepthExRTEntity;


    protected void Start() => Update();

    protected override void Update()
    {
        base.Update();
        SetCommand();
    }

    private void OnDisable() => RemoveCommand();

    protected override void OnValidate()
    {
        base.OnValidate();
        OnDisable();
    }

    private void SetCommand()
    {
        if(cmdAfterGenerator == null || cmdAfterMain == null)
        {
            RemoveCommand();

            // Camera Settings
            if (camera.depth > mainCamera.depth)
            {
                camera.depth = mainCamera.depth - 1;
            }
            depthExRTEntity = new RenderTexture(
                Screen.width / screenDivide, Screen.height / screenDivide,
                0, RenderTextureFormat.R8);
            infratedDepthExRTEntity = new RenderTexture(
                Screen.width / screenDivide, Screen.height / screenDivide,
                0, RenderTextureFormat.R8);
            camera.targetTexture = depthExRTEntity;


            int depthSwitchValueId = Shader.PropertyToID("_G_Depth");
            int directionValueId =  Shader.PropertyToID("_Direction");
            int depthExTexValue = Shader.PropertyToID("_Depth");
            int infratedDepthExTexValue = Shader.PropertyToID("_InfratedDepth");

            cmdAfterGenerator = new CommandBuffer();
            cmdAfterGenerator.name = "Generate Outline";
            cmdAfterGenerator.SetGlobalVector(directionValueId, new Vector4(1f / Screen.width, 1f / Screen.height));
            cmdAfterGenerator.Blit(depthExRTEntity, infratedDepthExRTEntity, procMaterial);
            cmdAfterGenerator.SetGlobalFloat(depthSwitchValueId, 0); // Set value for Composit
            camera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, cmdAfterGenerator);
            
            cmdAfterMain = new CommandBuffer();
            cmdAfterMain.name = "Composit Outline";
            cmdAfterMain.SetGlobalTexture(depthExTexValue, depthExRTEntity);
            cmdAfterMain.SetGlobalTexture(infratedDepthExTexValue, infratedDepthExRTEntity);
            cmdAfterMain.SetGlobalVector(directionValueId, new Vector4(1f / Screen.width , 1f / Screen.height));
            cmdAfterMain.Blit(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget, compMaterial);
            cmdAfterMain.SetGlobalFloat(depthSwitchValueId, 1); // Set value for DepthEx
            mainCamera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, cmdAfterMain);
        }
    }

    private void RemoveCommand()
    {
        if(camera != null)
        {
            camera.targetTexture = null;
            camera.RemoveAllCommandBuffers();
        }
        
        if(mainCamera != null)
        {
            mainCamera.RemoveAllCommandBuffers();
        }
        cmdAfterGenerator = null;
        cmdAfterMain = null;

        if(depthExRTEntity != null)
        {
            depthExRTEntity.Release();
            depthExRTEntity = null;
        }
        if(infratedDepthExRTEntity != null)
        {
            infratedDepthExRTEntity.Release();
            infratedDepthExRTEntity = null;
        }
    }
}
