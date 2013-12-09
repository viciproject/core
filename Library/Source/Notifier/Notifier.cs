#region License
//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2012 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Vici.Core.Notifications
{
    public delegate void NotificationDelegate(Notification notification);
    public delegate void NotificationDelegate<T>(Notification<T> notification);

    public class Notifier
    {
        private class Subscription : ISubscription
        {
            private readonly Notifier _notifier;
            private readonly string _pattern;
            private List<Notification> _queuedNotificications;
            public NotificationDelegate Method { get; private set; }

            protected Subscription(Notifier notifier, string pattern)
            {
                _notifier = notifier;
                _pattern = pattern;
            }

            public Subscription(Notifier notifier, string pattern, NotificationDelegate method)
            {
                _notifier = notifier;
                _pattern = pattern;
                Method = method;
            }

            public void Unsubscribe()
            {
                _notifier.Unsubscribe(this);
            }

            public void Dispose()
            {
                Unsubscribe();
            }

            public bool Matches(string name)
            {
                if (_pattern == null)
                    return true;

                if (name == null)
                    return false;

                if (name == _pattern)
                    return true;

                return Regex.IsMatch(name, '^' + _pattern + '$');
            }

            public IEnumerable<Notification> GetNotifications()
            {
                try
                {
                    return _queuedNotificications.ToArray();
                }
                finally
                {
                    _queuedNotificications.Clear();
                }
            }

            public void EnqueueNotification(Notification notification)
            {
                if (_queuedNotificications == null)
                    _queuedNotificications = new List<Notification>();

                _queuedNotificications.Add(notification);
            }
        }

        private class Subscription<T> : Subscription, ISubscription<T>
        {
            private readonly NotificationDelegate<T> _method;

            public Subscription(Notifier notifier, string name, NotificationDelegate<T> method)
                : base(notifier, name)
            {
                _method = method;
            }

            public new NotificationDelegate<T> Method
            {
                get { return _method; }
            }

            public new IEnumerable<Notification<T>> GetNotifications()
            {
                return base.GetNotifications().Cast<Notification<T>>();
            }
        }

        private readonly List<Subscription> _subscriptions = new List<Subscription>();
        public static Notifier Default { get; private set; }

        static Notifier()
        {
            Default = new Notifier();
        }


        public void Post(string name)
        {
            Post(name,null,null);
        }

        public void Post(string name, object payload)
        {
            Post(name,payload,null);
        }

        public void Post(string name, object payload, object sender)
        {
            Notification notification = new Notification(sender, name, payload);

            foreach (Subscription subscription in _subscriptions)
            {
                if (subscription.Matches(name))
                {
                    var method = subscription.Method;

                    if (method == null)
                        subscription.EnqueueNotification(notification);
                    else
                        method(notification);
                }
            }
        }

        public void Post<T>(string name, T payload, object sender)
        {
            Notification<T> notification = new Notification<T>(sender,name,payload);

            foreach (Subscription subscription in _subscriptions)
            {
                if (subscription.Matches(name))
                {
                    if (subscription is Subscription<T>)
                    {
                        NotificationDelegate<T> method = ((Subscription<T>) subscription).Method;

                        if (method == null)
                            subscription.EnqueueNotification(notification);
                        else
                            method(notification);
                    }
                    else
                    {
                        if (subscription.GetType() == typeof (Subscription))
                        {
                            NotificationDelegate method = subscription.Method;

                            if (method == null)
                                subscription.EnqueueNotification(notification);
                            else
                                method(notification);
                        }
                    }
                }
            }
        }

        public void Post<T>(string name, T payload)
        {
            Post(name,payload,null);
        }

        public void Post<T>(string name)
        {
            Post(name, default(T), null);
        }

        public ISubscription Subscribe(string name , NotificationDelegate method)
        {
            Subscription subscription = new Subscription(this, name, method);

            _subscriptions.Add(subscription);

            return subscription;
        }

        public ISubscription<T> Subscribe<T>(string name, NotificationDelegate<T> method)
        {
            Subscription<T> subscription = new Subscription<T>(this,name,method);

            _subscriptions.Add(subscription);

            return subscription;
        }

        public ISubscription<T> Subscribe<T>(NotificationDelegate<T> method)
        {
            return Subscribe(null,method);
        }

        public void Unsubscribe(ISubscription subscription)
        {
            _subscriptions.RemoveAll(s => subscription == s);
        }
    }

}
