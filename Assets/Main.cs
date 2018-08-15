using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Globalization;
using CustomRenderer;
using static Unity.Mathematics.math;
using Unity.Mathematics;
public class RenderTriangle {
  
    public float3[] points;
    public float lightValue;
    public Color faceColor;
    public RenderTriangle(){
        points = new float3[3];
        lightValue = 0f;
    }
    public void Log(){
        Debug.Log(points[0] + ", " + points[1] + ", " + points[2]);
    }
    public float averageX
    {
        get
        {
            return (points[0].x + points[1].x + points[2].x) / 3f;
        }
    }
    public float averageY
    {
        get
        {
            return (points[0].y + points[1].y + points[2].y) / 3f;
        }
    }
    public float averageZ {
        get
        {
            return (points[0].z + points[1].z + points[2].z) / 3f;
        }
    }
    public float3 averagePos {

        get{
            return (points[0] + points[1] + points[2])/ 3f;
        }
    }
}
public class CustomMesh
{
    public List<float3> verts;
    public List<int> tris;
    public List<Color> triColors;
    public float3 position, rotation;
    public CustomMesh(List<float3> verts, List<int> tris)
    {
        this.verts = verts;
        this.tris = tris;

    }

    public bool loadFromObjectFile(string path){
        Color vertexColor = Color.white;
        triColors = new List<Color>();
        verts = new List<float3>();
        tris = new List<int>();
        using(StreamReader stream = File.OpenText(path)){
            while(!stream.EndOfStream){
                string currentLine = stream.ReadLine();
                if (currentLine == "") continue;
                if (currentLine.Contains("BLACK")) vertexColor = Color.black;
                else if (currentLine.Contains("WHITE")) vertexColor = Color.white;
                else if (currentLine.Contains("BLUE")) vertexColor = new Color(0,.8f,1);
                else if (currentLine.Contains("RED")) vertexColor = Color.red;
                else if (currentLine.Contains("SKIN")) vertexColor = new Color(1, 0.8f, 0.49f, 1);
                else if (currentLine.Contains("HAIR")) vertexColor = new Color(0.54f,0,0,1);
                if(currentLine.Substring(0,1) == "v"){
                    float3 vertex = float3(0);
                    double result;
                    currentLine = currentLine.Substring(currentLine.IndexOf(" ") + 1);
                    double.TryParse(currentLine.Substring(0, currentLine.IndexOf(" ")), out result);
                    vertex.x = (float)result;

                    currentLine = currentLine.Substring(currentLine.IndexOf(" ") + 1);
                    double.TryParse(currentLine.Substring(0, currentLine.IndexOf(" ")), out result);
                    vertex.y = (float)result;

                    currentLine = currentLine.Substring(currentLine.IndexOf(" ") + 1);
                    double.TryParse(currentLine.IndexOf(" ") < 0 ? currentLine : currentLine.Substring(0, currentLine.IndexOf(" ")), out result);
                    vertex.z = (float)result;
                    verts.Add(vertex);
                }
                if (currentLine.Substring(0, 1) == "f")
                {
                    int[] triangle = new int[3];
                    currentLine = currentLine.Substring(currentLine.IndexOf(" ") + 1);
                    int.TryParse(currentLine.Substring(0, currentLine.IndexOf(" ")), out triangle[0]);
                    triangle[0] -= 1;
                    currentLine = currentLine.Substring(currentLine.IndexOf(" ") + 1);
                    int.TryParse(currentLine.Substring(0, currentLine.IndexOf(" ")), out triangle[1]);
                    triangle[1] -= 1;
                    currentLine = currentLine.Substring(currentLine.IndexOf(" ") + 1);
                    int.TryParse(currentLine.IndexOf(" ") < 0 ? currentLine : currentLine.Substring(0, currentLine.IndexOf(" ")), out triangle[2]);
                    triangle[2] -= 1;
                    tris.AddRange(triangle);
                    triColors.Add(vertexColor);
                }


            }

        }
        return true;
    }
}

public class Main : MonoBehaviour {
    public Material material;
    public Texture2D screentex;
    public int width, height;
    CustomMesh objMesh;
    float4x4 projMat, matRotX,matRotY, matRotZ,rotMat;
    float3 vCamera,  vLookDir;
    Vector3 cameraRotation;
    public string objPath;
	// Use this for initialization

    void SaveScreenAsTexture(){
        byte[] screenTextureBytes = screentex.EncodeToPNG();
        System.DateTime currentTime = System.DateTime.Now;
        File.WriteAllBytes(Application.dataPath + "/TestScreenshots/Screen Shot " + currentTime.ToString("yyyy'-'MM'-'dd 'at 'HH'.'mm'.'ss tt") + ".png", screenTextureBytes);
    }

   
    void ClearScreen(){

        screentex.SetPixels(Enumerable.Repeat<Color>(Color.black, width * height).ToArray<Color>());
        screentex.Apply();  
    }
    void Start()
    {
        screentex = new Texture2D(width, height);
        material.mainTexture = screentex;
        ClearScreen();
        RenderInit();
        //vecTrianglesToRaster.Clear();
        // RenderCustomMesh(objMesh);
        // RenderVertices();
    }
    void RenderInit(){
        objMesh = new CustomMesh(null, null); 
        if(objMesh.loadFromObjectFile(Application.dataPath + objPath)){
            Debug.Log(".obj mesh successfuly loaded.");
        }else{
            Debug.LogError(".obj mesh failed to import.");
        }
        objMesh.position = new float3(0, 0, 5);
        objMesh.rotation = new float3(0, 0, 0);
        float fNear = 0.1f;
        float fFar = 1000.0f;
        float fFov = 60f;
        float fAspectRatio = (float)height / (float)width;
        projMat = RenderMath.Matrix_MakeProjection(fFov, fAspectRatio, fNear, fFar);

    }
   
    // Update is called once per frame
    List<RenderTriangle> vecTrianglesToRaster = new List<RenderTriangle>();
    void Update()
    {

        if (Input.GetKey(KeyCode.UpArrow))
        {
            vCamera.y += 8.0f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            vCamera.y -= 8.0f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            vCamera.x -= 8.0f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            vCamera.x += 8.0f * Time.deltaTime;
        }

        float3 vForward = vLookDir * (8.0f * Time.deltaTime);

        if (Input.GetKey(KeyCode.W))
        {
            vCamera += vForward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            vCamera -= vForward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            cameraRotation.y -= 2f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            cameraRotation.y += 2f * Time.deltaTime;
        }


        ClearScreen();
         vecTrianglesToRaster.Clear();
        //objMesh.rotation.x += Time.deltaTime * Mathf.Rad2Deg;
        //objMesh.rotation.z += Time.deltaTime * 0.5f * Mathf.Rad2Deg; // Uncomment to spin me right round baby right round
        RenderCustomMesh(objMesh);
        RenderVertices();
        if(Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.S)){
            SaveScreenAsTexture();
        }
    
    }
    void RenderVertices(){
        

        vecTrianglesToRaster = vecTrianglesToRaster.OrderByDescending(a => a.averageZ).ToList();

        foreach (RenderTriangle tri in vecTrianglesToRaster)
        {
           
          // tri.lightValue = 1f;
            Rasterizer.FillTriangle(tri.points[0], tri.points[1], tri.points[2], new Color(tri.faceColor.r * tri.lightValue, tri.faceColor.g * tri.lightValue, tri.faceColor.b * tri.lightValue, 1),screentex);
             //Rasterizer.DrawTriangle(tri.points[0], tri.points[1], tri.points[2], Color.black,screentex);

        }
        screentex.Apply();

    }

    void RenderCustomMesh(CustomMesh renderMesh){
        float3 rotation = renderMesh.rotation * Mathf.Deg2Rad;

        rotMat = RenderMath.Matrix_MakeRotation(float3(cameraRotation.x,0,cameraRotation.z));
        RenderTriangle triProjected, triTransformed, triViewed;
        // float outputColor = 0f;
        float4x4 transMat = RenderMath.Matrix_MakeTranslation(renderMesh.position);
        float4x4 worldMat = RenderMath.Matrix_MakeIdentity();
        worldMat = rotMat;
        worldMat = RenderMath.Matrix_MultiplyMatrix(worldMat, transMat);
        float3 vUp = float3(0, 1, 0);
        float3 vTarget = float3(0,0,1);
        float4x4 cameraRotMat = RenderMath.Matrix_MakeRotation(float3(0, cameraRotation.y, 0));
        vLookDir = RenderMath.Matrix_MultiplyVector(vTarget,cameraRotMat);
        vTarget = vCamera + vLookDir;
        float4x4 cameraMat = RenderMath.Matrix_PointAt(vCamera, vTarget, vUp);
        float4x4 viewMat = RenderMath.Matrix_QuickInverse(cameraMat);
        for (int i = 0; i < renderMesh.tris.Count; i += 3)
        {
            triProjected = new RenderTriangle();
            triTransformed = new RenderTriangle();
            triViewed = new RenderTriangle();
            triTransformed.points[0] = RenderMath.Matrix_MultiplyVector(renderMesh.verts[renderMesh.tris[i]], worldMat);
            triTransformed.points[1] = RenderMath.Matrix_MultiplyVector(renderMesh.verts[renderMesh.tris[i + 1]], worldMat);
            triTransformed.points[2] = RenderMath.Matrix_MultiplyVector(renderMesh.verts[renderMesh.tris[i + 2]], worldMat);


            // triTransformed.Log();


            float3 normal, line1, line2;
            line1 = triTransformed.points[1] - triTransformed.points[0];
            line2 = triTransformed.points[2] - triTransformed.points[0];
            normal = cross(line1, line2);
            normal = normalize(normal);
            //Debug.DrawRay(new Vector3(triTransformed.averagePos.x,triTransformed.averagePos.y,triTransformed.averagePos.z), new Vector3(normal.x,normal.y,normal.z)); //show mesh normals in scene view;
            if (dot(normal, triTransformed.points[0] - vCamera) < 0f)
            {
                float3 light_direction = float3(0, 1, -1);
                light_direction = RenderMath.Vector_Normalize(light_direction);
                triViewed.lightValue = Mathf.Max(0.1f, dot(light_direction, normal));
                triViewed.faceColor = (renderMesh.triColors.Count * 3 == renderMesh.tris.Count ? renderMesh.triColors[i / 3] : Color.white);



                triViewed.points[0] = RenderMath.Matrix_MultiplyVector(triTransformed.points[0], viewMat);
                triViewed.points[1] = RenderMath.Matrix_MultiplyVector(triTransformed.points[1], viewMat);
                triViewed.points[2] = RenderMath.Matrix_MultiplyVector(triTransformed.points[2], viewMat);

                int nClippedTriangles = 0;
                RenderTriangle[] clipped = new RenderTriangle[2];
                clipped[0] = new RenderTriangle();
                clipped[1] = new RenderTriangle();
                nClippedTriangles = Triangle_ClipAgainstPlane(float3( 0.0f, 0.0f, 0.1f ), float3( 0.0f, 0.0f, 1.0f ), triViewed, ref clipped[0], ref clipped[1]);

                for (int n = 0; n < nClippedTriangles; n++)
                {

                    triProjected.lightValue = clipped[n].lightValue;
                    triProjected.faceColor = clipped[n].faceColor;
                    triProjected.points[0] = RenderMath.Matrix_MultiplyVector(triViewed.points[0], projMat);
                    triProjected.points[1] = RenderMath.Matrix_MultiplyVector(triViewed.points[1], projMat);
                    triProjected.points[2] = RenderMath.Matrix_MultiplyVector(triViewed.points[2], projMat);

                    triProjected.points[0] = normalize(triProjected.points[0]);
                    triProjected.points[1] = normalize(triProjected.points[1]);
                    triProjected.points[2] = normalize(triProjected.points[2]);
                    /*
                    triProjected.points[0].x *= -1.0f;
                    triProjected.points[1].x *= -1.0f;
                    triProjected.points[2].x *= -1.0f;
                    triProjected.points[0].y *= -1.0f;
                    triProjected.points[1].y *= -1.0f;
                    triProjected.points[2].y *= -1.0f;
*/
                    float3 vOffsetView = new float3(1, 1, 0);

                    triProjected.points[0] += vOffsetView;
                    triProjected.points[1] += vOffsetView;
                    triProjected.points[2] += vOffsetView;

                    triProjected.points[0].x *= 0.5f * (float)width;
                    triProjected.points[0].y *= 0.5f * (float)height;
                    triProjected.points[1].x *= 0.5f * (float)width;
                    triProjected.points[1].y *= 0.5f * (float)height;
                    triProjected.points[2].x *= 0.5f * (float)width;
                    triProjected.points[2].y *= 0.5f * (float)height;

                    vecTrianglesToRaster.Add(triProjected);

                }
            }

        }

    }

    float3 Vector_IntersectPlane(float3 plane_p, float3 plane_n, float3 lineStart, float3 lineEnd)
    {
        plane_n = normalize(plane_n);
        float plane_d = -dot(plane_n, plane_p);
        float ad = dot(lineStart, plane_n);
        float bd = dot(lineEnd, plane_n);
        float t = (-plane_d - ad) / (bd - ad);
        float3 lineStartToEnd = lineEnd - lineStart;
        float3 lineToIntersect = lineStartToEnd * t;
        return lineStart + lineToIntersect;
    }
    float dist(float3 p,float3 plane_p,float3 plane_n)

        {
            float3 n = normalize(p);
            return (plane_n.x* p.x + plane_n.y* p.y + plane_n.z* p.z - dot(plane_n, plane_p));
}

    int Triangle_ClipAgainstPlane(float3 plane_p, float3 plane_n, RenderTriangle in_tri, ref RenderTriangle out_tri1, ref RenderTriangle out_tri2)
    {
        // Make sure plane normal is indeed normal
        plane_n = normalize(plane_n);

        // Return signed shortest distance from point to plane, plane normal must be normalised
       
        // Create two temporary storage arrays to classify points either side of plane
        // If distance sign is positive, point lies on "inside" of plane
        float3[] inside_points = new float3[3]; int nInsidePointCount = 0;
        float3[] outside_points = new float3[3]; int nOutsidePointCount = 0;

        // Get signed distance of each point in triangle to plane
        float d0 = dist(in_tri.points[0],plane_p,plane_n);
        float d1 = dist(in_tri.points[1], plane_p, plane_n);
        float d2 = dist(in_tri.points[2], plane_p, plane_n);

        if (d0 >= 0) { inside_points[nInsidePointCount++] = in_tri.points[0]; }
        else { outside_points[nOutsidePointCount++] = in_tri.points[0]; }
        if (d1 >= 0) { inside_points[nInsidePointCount++] = in_tri.points[1]; }
        else { outside_points[nOutsidePointCount++] = in_tri.points[1]; }
        if (d2 >= 0) { inside_points[nInsidePointCount++] = in_tri.points[2]; }
        else { outside_points[nOutsidePointCount++] = in_tri.points[2]; }

        // Now classify triangle points, and break the input triangle into 
        // smaller output triangles if required. There are four possible
        // outcomes...

        if (nInsidePointCount == 0)
        {
            // All points lie on the outside of plane, so clip whole triangle
            // It ceases to exist

            return 0; // No returned triangles are valid
        }

        if (nInsidePointCount == 3)
        {
            // All points lie on the inside of plane, so do nothing
            // and allow the triangle to simply pass through
            out_tri1 = in_tri;

            return 1; // Just the one returned original triangle is valid
        }

        if (nInsidePointCount == 1 && nOutsidePointCount == 2)
        {
            // Triangle should be clipped. As two points lie outside
            // the plane, the triangle simply becomes a smaller triangle

            // Copy appearance info to new triangle
            out_tri1.lightValue = in_tri.lightValue;
            out_tri1.faceColor = in_tri.faceColor;
            // The inside point is valid, so keep that...
            out_tri1.points[0] = inside_points[0];

            // but the two new points are at the locations where the 
            // original sides of the triangle (lines) intersect with the plane
            out_tri1.points[1] = Vector_IntersectPlane(plane_p, plane_n, inside_points[0], outside_points[0]);
            out_tri1.points[2] = Vector_IntersectPlane(plane_p, plane_n, inside_points[0], outside_points[1]);

            return 1; // Return the newly formed single triangle
        }

        if (nInsidePointCount == 2 && nOutsidePointCount == 1)
        {
            // Triangle should be clipped. As two points lie inside the plane,
            // the clipped triangle becomes a "quad". Fortunately, we can
            // represent a quad with two new triangles

            // Copy appearance info to new triangles
            out_tri1.lightValue = in_tri.lightValue;
            out_tri1.faceColor = in_tri.faceColor;
            out_tri2.lightValue = in_tri.lightValue;
            out_tri2.faceColor = in_tri.faceColor;



            // The first triangle consists of the two inside points and a new
            // point determined by the location where one side of the triangle
            // intersects with the plane
            out_tri1.points[0] = inside_points[0];
            out_tri1.points[1] = inside_points[1];
            out_tri1.points[2] = Vector_IntersectPlane(plane_p, plane_n, inside_points[0], outside_points[0]);

            // The second triangle is composed of one of he inside points, a
            // new point determined by the intersection of the other side of the 
            // triangle and the plane, and the newly created point above
            out_tri2.points[0] = inside_points[1];
            out_tri2.points[1] = out_tri1.points[2];
            out_tri2.points[2] = Vector_IntersectPlane(plane_p, plane_n, inside_points[1], outside_points[0]);

            return 2; // Return two newly formed triangles which form a quad
        }
        return 0;
    }

   
   
}
