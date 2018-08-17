﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class LBClientSender : MonoBehaviour
{

    public int ClientPort = 0;
    public int LBServerPort = 2000;
    public string LBServerIP = "10.0.0.215";

    private readonly byte[] header = System.Array.ConvertAll(new char[] { 'L', 'e', 'd', 'B', 'u', 'r', 'n' }, System.Convert.ToByte);

    private const int HEADER_LENGTH = 24;
    private const byte PROTOCOL_VERSION = 0;
    private uint m_lastFrameId;

    private UdpClient m_udpClient;
    private IPEndPoint m_targetIP;

    // this dictionary stores all the segments that we are working with.
    // the LBClientSender library enables the user to change the segments on the fly, therefore
    // it allows this dictionary to grow and shrink on the fly.
    // However, we do not expect it to happen frequently, thus this variable stores the current
    // segments that we are working with.
    // the key to the dictionary is a 32bit number which is the union of the strip id and pixel address
    private Dictionary<uint, byte[]> m_allSegments = new Dictionary<uint, byte[]>();

    public void SetData(ushort stripId, ushort pixelAddressInStrip, Color32[] colors)
    {
        // check if a new segment needs to be added
        uint dictKey = ((uint)stripId << 16) | (uint)pixelAddressInStrip;
        
        byte[] msgByteArr;
        if (!m_allSegments.TryGetValue(dictKey, out msgByteArr))
        {
            createNewSegment(dictKey, stripId, pixelAddressInStrip, colors.Length);
            msgByteArr = m_allSegments[dictKey];
        }

        // check if the size changed
        int expectedLength = HEADER_LENGTH + 3 * colors.Length;
        if (msgByteArr.Length != expectedLength)
        {
            createNewSegment(dictKey, stripId, pixelAddressInStrip, colors.Length);
            msgByteArr = m_allSegments[dictKey];
        }

        // copy the rgb values to the internal buffer. 
        // it will be cached and sent later after all segments are updated with new frame data
        for (int i = 0; i < colors.Length; i++)
        {
            int pixelBufferIndex = HEADER_LENGTH + 3 * i;
            msgByteArr[pixelBufferIndex + 0] = colors[i].r;
            msgByteArr[pixelBufferIndex + 1] = colors[i].g;
            msgByteArr[pixelBufferIndex + 2] = colors[i].b;
        }
    }

    void Start()
    {
        m_lastFrameId = (uint)UnityEngine.Random.Range(0, 10000);
        m_udpClient = new UdpClient(ClientPort);
        ClientPort = (m_udpClient.Client.LocalEndPoint as IPEndPoint).Port;
        m_targetIP = new IPEndPoint(IPAddress.Parse(LBServerIP), LBServerPort);
        m_udpClient.Connect(m_targetIP);
    }

    void Update()
    {
        if (m_udpClient == null)
            return;
        m_lastFrameId++;
        foreach (byte[] segmentByteArray in m_allSegments.Values)
        {
            writeUint32ToArray(segmentByteArray, 8, m_lastFrameId);
            // m_udpClient.Send(segmentByteArray, segmentByteArray.Length, m_targetIP);
            m_udpClient.Send(segmentByteArray, segmentByteArray.Length);
        }
    }

    private void createNewSegment(uint dictKey, ushort stripId, ushort pixelAddressInStrip, int numberOfPixels)
    {

        m_allSegments[dictKey] = new byte[HEADER_LENGTH + 3 * numberOfPixels];
        byte[] msgByteArr = m_allSegments[dictKey];

        // Protocol Definition
        // msgByteArr[0] = System.Convert.ToByte('L');
        // msgByteArr[1] = System.Convert.ToByte('e');
        // msgByteArr[2] = System.Convert.ToByte('d');
        // msgByteArr[3] = System.Convert.ToByte('B');
        // msgByteArr[4] = System.Convert.ToByte('u');
        // msgByteArr[5] = System.Convert.ToByte('r');
        // msgByteArr[6] = System.Convert.ToByte('n');
        Buffer.BlockCopy(header, 0, msgByteArr, 0, 7);
        writeUint8ToArray(msgByteArr, 7, PROTOCOL_VERSION);

        // Frame definition
        writeUint32ToArray(msgByteArr, 8, 0); // frame id
        writeUint32ToArray(msgByteArr, 12, (ushort)m_allSegments.Count); // total number of segments in frame

        // segment definition
        writeUint32ToArray(msgByteArr, 16, (ushort)(m_allSegments.Count - 1)); // current segment id
        writeUint16ToArray(msgByteArr, 20, stripId); // physical strip number
        writeUint16ToArray(msgByteArr, 22, pixelAddressInStrip); // first pixel address in strip

        updateSegmentsCount();
    }

    private void updateSegmentsCount()
    {
        ushort totalNumberOfSegments = (ushort)m_allSegments.Count;
        ushort segmentsIndexForLoop = 0;

        var enumerator = m_allSegments.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var segmentByteArray = enumerator.Current.Value;
            writeUint32ToArray(segmentByteArray, 12, totalNumberOfSegments); // total number of segments in frame
            writeUint32ToArray(segmentByteArray, 16, segmentsIndexForLoop); // current segment id
            segmentsIndexForLoop++;
        }

        // foreach (byte[] segmentByteArray in m_allSegments.Values)
        // {
        //     writeUint32ToArray(segmentByteArray, 12, totalNumberOfSegments); // total number of segments in frame
        //     writeUint32ToArray(segmentByteArray, 16, segmentsIndexForLoop); // current segment id
        //     segmentsIndexForLoop++;
        // }
    }

    private static void writeUint8ToArray(byte[] arr, int locInArray, byte valToWrite)
    {
        arr[locInArray] = (byte)valToWrite;
    }

    private static void writeUint16ToArray(byte[] arr, int locInArray, ushort valToWrite)
    {
        arr[locInArray + 0] = (byte)((valToWrite) & 0xff);
        arr[locInArray + 1] = (byte)((valToWrite >> 8) & 0xff);
        // Buffer.BlockCopy(BitConverter.GetBytes(valToWrite), 0, arr, locInArray, sizeof(ushort));
    }

    private static void writeUint32ToArray(byte[] arr, int locInArray, uint valToWrite)
    {
        arr[locInArray + 0] = (byte)((valToWrite) & 0xff);
        arr[locInArray + 1] = (byte)((valToWrite >> 8) & 0xff);
        arr[locInArray + 2] = (byte)((valToWrite >> 16) & 0xff);
        arr[locInArray + 3] = (byte)((valToWrite >> 24) & 0xff);
        // Buffer.BlockCopy(BitConverter.GetBytes(valToWrite), 0, arr, locInArray, sizeof(uint));
    }
}
