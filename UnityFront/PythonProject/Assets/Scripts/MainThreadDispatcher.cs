using UnityEngine;
using System.Collections.Generic;
using System;

/**
 * Socket catches event with threads!
 * Only the main thread can modify the scene ui.
 */
public class MainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> m_executionQueue = new Queue<Action>();
    private static MainThreadDispatcher m_instance;

    private void Awake()
    {
        m_instance = this;
    }

    public static MainThreadDispatcher Instance => m_instance;

    public void Enqueue(Action action)
    {
        lock (m_executionQueue)
        {
            m_executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        while (m_executionQueue.Count > 0)
        {
            Action action;
            lock (m_executionQueue)
            {
                action = m_executionQueue.Dequeue();
            }
            action?.Invoke();
        }
    }
}