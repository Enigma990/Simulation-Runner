using UnityEngine;

public class RewindTime
{
    private Vector3 m_position;
    public Vector3 Position { get { return m_position; } }

    private Quaternion m_rotation;
    public Quaternion Rotation { get { return m_rotation; } }

    public RewindTime(Vector3 position, Quaternion rotation)
    {
        m_position = position;
        m_rotation = rotation;
    }
}
