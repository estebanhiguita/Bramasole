using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Tatacoa
{
	public abstract class Promise<A,B> : Functor<B> {

		public abstract Promise<A,C> FMap<C> (Func<B,C> g);
		public abstract Promise<A,B> FMap (Func<B,B> g);
		public abstract Promise<A,B> FMap (Action<B> g);

		public abstract Promise<A,B> Resolve (A val);
		public abstract Promise<A,B> Reject();

		public abstract B value { get; }
		public abstract Func<A,B> function { get;}

		Functor<C> Functor<B>.FMap<C> (Func<B, C> f) {
			return FMap (f);
		}

		public Functor<B> XMap (Func<Exception, Exception> fx)
		{
			throw new NotImplementedException ();
		}

		public abstract bool isBroken { get;}
		public abstract bool isPending { get;}
		public abstract bool isResolved { get;}

	}

	public class Pending<A,B> : Promise<A,B> {

		Func<A,B> f;

		public Pending (Func<A,B> g) {
			f = g;	
		}

		public override Promise<A,C> FMap<C> (Func<B, C> g)
		{
			return new Pending<A,C> (Fn.Compose (g, f));
		}

		public override Promise<A,B> FMap (Func<B, B> g)
		{
			f = Fn.Compose (g, f);
			return this;
		}

		public override Promise<A,B> FMap (Action<B> g)
		{
			f = Fn.Compose (g.ToFunc (), f);
			return this;
		}

		public override Promise<A,B> Resolve (A val)
		{
			return val == null ? new Broken<A,B>() as Promise<A,B> : new Resolved<A,B> (f (val)) as Promise<A,B>;
		}

		public override Promise<A,B> Reject ()
		{
			return new Broken<A,B> ();
		}

		public override B value {
			get {
				return default(B);
			}
		}

		public override Func<A, B> function {
			get {
				return f;
			}
		}

		public override bool isBroken {
			get {
				return false;
			}
		}

		public override bool isPending {
			get {
				return true;
			}
		}

		public override bool isResolved {
			get {
				return false;
			}
		}
	}

	public class Resolved<A,B> : Promise<A,B> {

		B val;

		public Resolved (B value) {
			val = value;	
		}

		public override Promise<A,C> FMap<C> (Func<B, C> g)
		{
			return new Resolved<A,C> (g (val));
		}

		public override Promise<A,B> FMap (Func<B, B> g)
		{
			val = g (val);
			return this;
		}

		public override Promise<A,B> FMap (Action<B> g)
		{
			g (val);
			return this;
		}

		public override Promise<A,B> Resolve (A val)
		{
			return this;
		}

		public override Promise<A,B> Reject ()
		{
			return this;
		}

		public override B value {
			get {
				return val;
			}
		}

		public override Func<A, B> function {
			get {
				return default(Func<A, B>);
			}
		}

		public override bool isBroken {
			get {
				return false;
			}
		}
		
		public override bool isPending {
			get {
				return false;
			}
		}
		
		public override bool isResolved {
			get {
				return true;
			}
		}
	}

	public class Broken<A,B> : Promise<A,B> {

		public override Promise<A,C> FMap<C> (Func<B, C> g)
		{
			return new Broken<A,C> ();
		}

		public override Promise<A,B> FMap (Func<B, B> g)
		{
			return this;
		}

		public override Promise<A,B> FMap (Action<B> g)
		{
			return this;
		}

		public override Promise<A,B> Resolve (A val)
		{
			return this;
		}

		public override Promise<A,B> Reject ()
		{
			return this;
		}

		public override B value {
			get {
				return default(B);
			}
		}

		public override Func<A, B> function {
			get {
				return default(Func<A, B>);
			}
		}

		public override bool isBroken {
			get {
				return true;
			}
		}
		
		public override bool isPending {
			get {
				return false;
			}
		}
		
		public override bool isResolved {
			get {
				return false;
			}
		}
	}

	public interface PromiseAction {
		
		PromiseAction FMap (Action g);
		PromiseAction resolve ();
		PromiseAction reject ();
	}

	public class Pending : PromiseAction {
		Action f;

		public Pending (Action g) {
			f = g;
		}

		public PromiseAction FMap (Action g)
		{
			f = Fn.Compose (g, f);
			return this;
		}

		public PromiseAction resolve ()
		{
			f ();
			return new Resolved ();
		}

		public PromiseAction reject ()
		{
			return new Broken ();
		}
	}

	public class Resolved : PromiseAction {

		public PromiseAction FMap (Action g)
		{
			g ();
			return this;
		}

		public PromiseAction resolve ()
		{
			return this;
		}

		public PromiseAction reject ()
		{
			return this;
		}

	}

	public class Broken : PromiseAction {

		public PromiseAction FMap (Action g)
		{
			return this;
		}

		public PromiseAction resolve ()
		{
			return this;
		}

		public PromiseAction reject ()
		{
			return this;
		}
	}

	public class TPromise{
		private static TPromise _i;
		public static TPromise i {
			get {
				if (_i != null) _i = new TPromise();
				return _i;
			}
		}
		private TPromise (){}
	}

	public static partial class Fn {

		public static Promise<A,A> MakePromise<A>() {
			return new Pending<A,A> (Fn.Id<A>);
		}
		
		public static Promise<A,B> MakePromise<A,B> (Func<A,B> f) {
			return new Pending<A,B> (f);
		}
		
		public static Promise<A,A> MakePromise<A> (Action<A> f) {
			return new Pending<A,A> (f.ToFunc ());
		}
		
		public static Promise<A,A> MakePromise<A> (A val) {
			return val == null ? new Broken<A,A>() as Promise<A,A> : new Resolved<A,A> (val) as Promise<A,A>;
		}

		public static Promise<R,B> FMap<R,A,B> (Func<A,B> f, Promise<R,A> F) {
			return F.FMap (f);
		}

		public static Promise<R,A> FMap<R,A> (Action<A> f, Promise<R,A> F) {
			return F.FMap (f);
		}

		//FMap :: (a -> b) -> (Maybe a -> Maybe b)
		public static Func<Promise<R,A>,Promise<R,B>> FMap<R,A,B> (TPromise _, Func<A,B> f) {
			return F => F.FMap (f);
		}

		//FMap :: (a -> void) -> (Maybe a -> Maybe a)
		public static Func<Promise<R,A>,Promise<R,A>> FMap<R,A> (TPromise _, Action<A> f) {
			return F => F.FMap (f);
		}

		//FMap :: (a -> void) -> (Maybe a -> Maybe a)
		public static Func<Promise<A,A>,Promise<A,A>> FMap<A> (TPromise _, Action<A> f) {
			return F => F.FMap (f);
		}

		// Compose :: Promise b c -> Promise a b -> Promise a -> c
		public static Promise<A,C> Compose<A,B,C> (Promise<B,C> g, Promise<A,B> f) {
			if (g.isBroken || f.isBroken)
				return new Broken<A,C>();

			if (g.isResolved)
				return new Resolved<A,C> (g.value);

			if (f.isResolved)
				return new Resolved<A,C> (g.Resolve (f.value).value);

			return new Pending<A,C> ((g.function) .o (f.function));
		}

		public static Promise<A,C> o<A,B,C> (this Promise<B,C> g, Promise<A,B> f) {
			return Compose (g, f);
		}
	}
}