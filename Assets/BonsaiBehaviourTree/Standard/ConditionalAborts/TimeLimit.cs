﻿
using System.Text;
using Bonsai.Core;
using UnityEngine;

namespace Bonsai.Standard
{
  /// <summary>
  /// Sets a time limit for the child to finish executing.
  /// If the time is up, the decorator returns failure.
  /// </summary>
  [BonsaiNode("Conditional/", "Condition")]
  public sealed class TimeLimit : ConditionalAbort
  {
    [ShowAtRuntime]
    [UnityEngine.SerializeField]
    public Utility.Timer timer = new Utility.Timer();

    public override void OnStart()
    {
      timer.OnTimeout += OnTimeout;
    }

    public override void OnEnter()
    {
      Tree.AddTimer(timer);
      timer.Start();
      base.OnEnter();
    }

    public override void OnExit()
    {
      if (abortType == AbortType.Self)
        Tree.RemoveTimer(timer);
      base.OnExit();
    }

    public override bool Condition()
    {
      return abortType == AbortType.Self ? !timer.IsDone : timer.IsDone;
    }

    private void OnTimeout()
    {
      // Timer complete, notify abort.
      Tree.RemoveTimer(timer);
      Evaluate();
    }

    public override void Description(StringBuilder builder)
    {
      base.Description(builder);
      builder.AppendLine();
      builder.AppendFormat("Abort and fail after {0:0.00}s", timer.interval);
    }
  }
}