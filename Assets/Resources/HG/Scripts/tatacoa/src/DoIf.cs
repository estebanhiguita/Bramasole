using UnityEngine;
using System;
using System.Collections;

namespace Tatacoa 
{
	public abstract class DoIf<A> : Functor<A> {

		internal Func<bool> condition = () => false;
		internal Func<bool> guard = () => true;

		public bool truth {
			get {
				return condition() && guard();
			}
		}

		public abstract DoIf<B> FMap<B> (Func<A,B> f);
		public abstract DoIf<A> FMap (Action<A> f);
		public abstract DoIf<A> Do (Func<A> f);
		public abstract DoIf<A> Try ();
		public abstract DoIf<A> If (Func<bool> cond);
		public DoIf<A> If (bool cond) {
			return If (() => cond);
		}
		public abstract DoIf<A> Guard (Func<bool> cond);
		public abstract A value {get;}
		public abstract bool IsWaiting {get;}

		Functor<B> Functor<A>.FMap<B> (Func<A, B> f)
		{
			throw new System.NotImplementedException ();
		}

		public Functor<A> XMap (Func<Exception, Exception> fx)
		{
			throw new NotImplementedException ();
		}

		public static bool operator true (DoIf<A> m) {
			return !m.IsWaiting;
		}
		
		public static bool operator false (DoIf<A> m) {
			return m.IsWaiting;
		}
		
		public static bool operator ! (DoIf<A> m) {
			return m.IsWaiting;
		}

	}

	public class Waiting<A> : DoIf<A> {

		Func<A> fa;

		public Waiting () {
			fa = () => {
				throw new ArgumentException("Undefined Action");
				return default(A);
			};
		}

		public Waiting (Func<A> f) {
			fa = f;
		}

		public Waiting (A a) {
			fa = () => a;
		}

		public Waiting (Func<A> f, Func<bool> cond) {
			fa = f;
			condition = cond;
		}

		public Waiting (Func<A> f, Func<bool> cond, Func<bool> guard) {
			fa = f;
			condition = cond;
			this.guard = guard;
		}
		
		public Waiting (A a, Func<bool> cond) {
			fa = () => a;
			condition = cond;
		}

		public override DoIf<A> Do (Func<A> f) {
			this.fa = f;
			return Try ();
		}

		public override DoIf<B> FMap<B> (Func<A, B> f)
		{
			return new Waiting<B> (() => f (fa ()), condition, guard).Try ();
		}

		public override DoIf<A> FMap (Action<A> f)
		{
			return new Waiting<A> (() => f.ToFunc () (fa ()), condition, guard).Try();
		}

		public override DoIf<A> If (Func<bool> cond)
		{
			return new Waiting<A> (fa, () => cond() || condition(), guard).Try();
		}

		public override DoIf<A> Guard (Func<bool> cond)
		{
			return new Waiting<A> (fa, condition, () => cond () && guard ()).Try();
		}

		public override DoIf<A> Try ()
		{
			return truth ? new Done<A>(fa(), condition, guard) as DoIf<A> : this as DoIf<A>;
		}

		public override A value {
			get {
				return default(A);
			}
		}

		public override bool IsWaiting {
			get {
				return true;
			}
		}
	}

	public class Done<A> : DoIf<A> {

		A a;

		public Done (A val) {
			a = val;
		}

		public Done (A val, Func<bool> cond) {
			a = val;
			condition = cond;
		}

		public Done (A val, Func<bool> cond, Func<bool> guard) {
			a = val;
			condition = cond;
			this.guard = guard;
		}

		public override DoIf<B> FMap<B> (Func<A, B> f)
		{
			return new Waiting<A> (() => a, condition, guard).FMap (f).Try();
		}

		public override DoIf<A> FMap (Action<A> f)
		{
			return FMap (f.ToFunc ());
		}

		public override DoIf<A> Do (Func<A> f) {
			return new Waiting<A> (f, condition).Try();
		}

		public override DoIf<A> Try ()
		{
			return this;
		}

		public override DoIf<A> If (Func<bool> cond)
		{
			return new Done<A> (a, () => cond () || condition (), guard).Try();
		}

		public override DoIf<A> Guard (Func<bool> cond)
		{
			return new Done<A> (a, condition, () => cond() && guard()).Try();
		}

		public override A value {
			get {
				return a;
			}
		}

		public override bool IsWaiting {
			get {
				return false;
			}
		}
		
	}

	public abstract class DoIf {
		internal Func<bool> condition = () => false;
		
		public bool truth {
			get {
				return condition();
			}
		}
		
		public abstract DoIf FMap (Action g);
		public abstract DoIf Do (Action g);
		public abstract DoIf Try ();
		public abstract DoIf If (Func<bool> cond);
		public DoIf If (bool cond) {
			return If (() => cond);
		}
		public abstract bool IsWaiting {get;}

		public static bool operator true (DoIf m) {
			return ! m.IsWaiting;
		}
		
		public static bool operator false (DoIf m) {
			return m.IsWaiting;
		}
		
		public static bool operator ! (DoIf m) {
			return m.IsWaiting;
		}
	}

	public class Waiting : DoIf {
		Action f;

		public Waiting (bool cond) {
			f = () => {
				throw new ArgumentNullException("Action not defined");
			};
			condition = () => cond;
		}

		public Waiting (Action g) {
			f = g;
		}

		public Waiting (Action g, Func<bool> cond) {
			f = g;
			condition = cond;
		}

		public override DoIf FMap (Action g)
		{
			return new Waiting ((g) .o (f), condition).Try ();
		}

		public override DoIf Do (Action g)
		{
			f = g;
			return Try ();
		}

		public override DoIf Try ()
		{
			return truth ? new Done (f).Try() as DoIf : this as DoIf;
		}

		public override DoIf If (Func<bool> cond)
		{
			return new Waiting (f, () => cond () || condition ()).Try();
		}

		public override bool IsWaiting {
			get {
				return true;
			}
		}
	}

	public class Done : DoIf {

		public Done (Func<bool> cond){
			condition = cond;
		}
		public Done (Action f) {
			f ();
		}

		public override DoIf Try ()
		{
			return this;
		}

		public override DoIf FMap (Action g)
		{
			return new Waiting (g, condition).Try ();
		}

		public override DoIf Do (Action g)
		{
			return new Waiting (g).Try ();
		}

		public override DoIf If (Func<bool> cond)
		{
			return new Done (() => cond () || condition ()).Try();
		}

		public override bool IsWaiting {
			get {
				return false;
			}
		}
	}

	public static partial class Fn {

		public static DoIf<A> Do<A> (Func<A> f) {
			return new Waiting<A> (f);
		}

		public static DoIf Do (Action f) {
			return new Waiting (f);
		}

		public static DoIf<A> If<A> (Func<bool> cond) {
			return new Waiting<A> ().If (cond);
		}

		public static DoIf<A> If<A> (bool cond) {
			return new Waiting<A> ().If (() => cond);
		}

		public static DoIf If (Func<bool> cond) {
			return new Waiting (cond());
		}
		
		public static DoIf If (bool cond) {
			return new Waiting (cond);
		}
	}
}