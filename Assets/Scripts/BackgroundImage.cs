using UnityEngine;
 
 public class BackgroundImage : MonoBehaviour
 {
     public Texture2D texture;
     void OnPostRender()
     {
         GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
         Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
     }
 }