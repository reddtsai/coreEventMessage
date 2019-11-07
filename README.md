# eventMessage

[![Build status](https://ci.appveyor.com/api/projects/status/gyjqbgme79i9rkyx?svg=true)](https://ci.appveyor.com/project/reddtsai/coreeventmessage)  [![NetMQ NuGet version](https://img.shields.io/nuget/v/ReddEventMessage.svg)](https://www.nuget.org/packages/ReddEventMessage/)

This is a personal library project is used for communication between processes. Based on message queues pattern. Implementation message queues with [NetMQ](https://github.com/zeromq/netmq).

## Installation

You can download via

## Features

- Publish/Subscript Message

    ```C#
    using(var sub = new MessageSubscriber("localhost", 9101))
    {
        var handler = new TestSubscribeHandler();
        sub.SetReceiveHandleEvent(handler);
        sub.Listen();
        sub.Subscribe(Encoding.UTF8.GetBytes(topic));
    }

    using(var pub = new MessagePublisher(9101))
    {
        pub.Send(Encoding.UTF8.GetBytes(topic), Encoding.UTF8.GetBytes(message));
    }
    ```

- Pull/Push Message

    ```C#
    using(var push = new MessagePuller(9101))
    {
        var Handler = new TestPullHandler();
        Pull.SetReceiveHandleEvent(Handler);
        Pull.Listen();
    }

    using(var push = new MessagePusher("localhost", 9101))
    {
        push.Send(Encoding.UTF8.GetBytes("hello"));
    }
    ```
