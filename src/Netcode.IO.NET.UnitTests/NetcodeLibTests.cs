using Xunit;
using NetcodeIO.NET.Tests;
using System.Diagnostics;
using System;

namespace Netcode.IO.NET.UnitTests;

public class NetcodeLibTests
{
    [Fact(Timeout = 2000)]
    public void Test1()
    {

    }

    [Fact(Timeout = 2000)]
    public void TestSequence()
    {
        Tests.TestSequence();
    }

    [Fact(Timeout = 2000)]
    public void TestConnectToken()
    {
        Tests.TestConnectToken();
    }

    [Fact(Timeout = 2000)]
    public void TestChallengeToken()
    {
        Tests.TestChallengeToken();
    }

    [Fact(Timeout = 2000)]
    public void TestEncryptionManager()
    {
        Tests.TestEncryptionManager();
    }

    [Fact(Timeout = 2000)]
    public void TestReplayProtection()
    {
        Tests.TestReplayProtection();
    }

    [Fact(Timeout = 2000)]
    public void TestConnectionRequestPacket()
    {
        Tests.TestConnectionRequestPacket();
    }

    [Fact(Timeout = 2000)]
    public void TestConnectionDeniedPacket()
    {
        Tests.TestConnectionDeniedPacket();
    }

    [Fact(Timeout = 2000)]
    public void TestConnectionKeepAlivePacket()
    {
        Tests.TestConnectionKeepAlivePacket();
    }

    [Fact(Timeout = 2000)]
    public void TestConnectionChallengePacket()
    {
        Tests.TestConnectionChallengePacket();
    }

    [Fact(Timeout = 2000)]
    public void TestConnectionPayloadPacket()
    {
        Tests.TestConnectionPayloadPacket();
    }

    [Fact(Timeout = 2000)]
    public void TestConnectionDisconnectPacket()
    {
        Tests.TestConnectionDisconnectPacket();
    }

    [Fact(Timeout = 2000)]
    public void TestClientServerConnection()
    {
        Tests.TestClientServerConnection();
    }

    [Fact(Timeout = 2000)]
    public void TestClientServerKeepAlive()
    {
        Tests.TestClientServerKeepAlive();
    }

    [Fact(Timeout = 2000)]
    public void TestClientServerMultipleClients()
    {
        Tests.TestClientServerMultipleClients();
    }

    [Fact(Timeout = 2000)]
    public void TestClientServerMultipleServers()
    {
        Tests.TestClientServerMultipleServers();
    }

    [Fact(Timeout = 2000)]
    public void TestConnectTokenExpired()
    {
        Tests.TestConnectTokenExpired();
    }

    [Fact(Timeout = 2000)]
    public void TestInvalidConnectToken()
    {
        Tests.TestClientInvalidConnectToken();
    }

    [Fact(Timeout = 2000)]
    public void TestConnectionTimeout()
    {
        Tests.TestConnectionTimeout();
    }

    [Fact(Timeout = 2000)]
    public void TestChallengeResponseTimeout()
    {
        Tests.TestChallengeResponseTimeout();
    }

    [Fact(Timeout = 2000)]
    public void TestConnectionRequestTimeout()
    {
        Tests.TestConnectionRequestTimeout();
    }

    [Fact(Timeout = 2000)]
    public void TestConnectionDenied()
    {
        Tests.TestConnectionDenied();
    }

    [Fact(Timeout = 2000)]
    public void TestClientSideDisconnect()
    {
        Tests.TestClientSideDisconnect();
    }

    [Fact(Timeout = 2000)]
    public void TestServerSideDisconnect()
    {
        Tests.TestServerSideDisconnect();
    }

    [Fact(Timeout = 2000)]
    public void TestClientReconnect()
    {
        Tests.TestReconnect();
    }

    [Fact(Timeout = 2000)]
    public void SoakConnectionTests()
    {
        const int soakTime = 1000 * 60 * 10;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        int iterations = 0;
        while (sw.ElapsedMilliseconds < soakTime)
        {
            Console.WriteLine("=== RUN " + iterations + " ===");

            Tests.TestClientServerConnection();
            Tests.TestClientServerKeepAlive();
            Tests.TestClientServerKeepAlive();
            Tests.TestClientServerMultipleClients();
            Tests.TestClientServerMultipleServers();
            Tests.TestConnectTokenExpired();
            Tests.TestClientInvalidConnectToken();
            Tests.TestConnectionTimeout();
            Tests.TestChallengeResponseTimeout();
            Tests.TestConnectionRequestTimeout();
            Tests.TestConnectionDenied();
            Tests.TestClientSideDisconnect();
            Tests.TestServerSideDisconnect();
            Tests.TestReconnect();

            iterations++;
        }

        sw.Stop();
    }

    [Fact(Timeout = 2000)]
    public void SoakClientServerRandomConnection()
    {
        Tests.SoakTestClientServerConnection(30);
    }
}