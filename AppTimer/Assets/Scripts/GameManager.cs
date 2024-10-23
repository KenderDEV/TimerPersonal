using System;
using UnityEngine;
using System.Runtime.InteropServices;


public class GameManager : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] Color textColor;
    [Header("Sprites")]
    [SerializeField] Sprite btnsprite;

    // Importar la función SetForegroundWindow
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    // Importar la función GetActiveWindow
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    // Importar la función FlashWindowEx
    [DllImport("user32.dll")]
    private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

    // Estructura para FlashWindowEx
    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO
    {
        public uint cbSize;
        public IntPtr hwnd;
        public uint dwFlags;
        public uint uCount;
        public uint dwTimeout;
    }

    // Constantes para el parpadeo
    const uint FLASHW_ALL = 3;
    const uint FLASHW_TIMERNOFG = 12;

    // Método para traer la ventana al frente
    public static void BringWindowToFront()
    {
        IntPtr windowHandle = GetActiveWindow();
        if (windowHandle != IntPtr.Zero)
        {
            SetForegroundWindow(windowHandle);
        }
    }

    // Método para hacer que la ventana parpadee en la barra de tareas
    public static void FlashWindow()
    {
        IntPtr windowHandle = GetActiveWindow();
        if (windowHandle != IntPtr.Zero)
        {
            FLASHWINFO flashInfo = new FLASHWINFO
            {
                cbSize = Convert.ToUInt32(Marshal.SizeOf(typeof(FLASHWINFO))),
                hwnd = windowHandle,
                dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG,
                uCount = uint.MaxValue, // Parpadeo indefinido
                dwTimeout = 0 // Valor por defecto del sistema
            };

            FlashWindowEx(ref flashInfo);
        }
    }
    public Sprite buttonSprite()
    {
        return btnsprite;
    }
}
