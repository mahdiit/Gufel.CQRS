﻿namespace Gufel.Dispatcher.Base.Dispatcher;

public interface IRequest : IBaseRequest { }

public interface IRequest<out TResponse> : IBaseRequest { }

public interface IBaseRequest { }