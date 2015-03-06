
using System;
using System.Collections;

namespace Tatacoa
{
	public abstract class Either<A,B> : Monad<B> {

		public abstract Either<A,C> Bind<C> (Func<B,Either<A,C>> f);
		public abstract Either<C,B> BindL<C> (Func<A,Either<C,B>> f);

		public abstract Either<A,C> FMap<C> (Func<B,C> f);

		public Either<A,B> FMap (Action<B> f) {
			return FMap (f.ToFunc ());
		}
		public Either<A,B> FMap (Action f) {
			return FMap (f.ToFunc<B> ());
		}

		public abstract Either<C,B> MapL<C> (Func<A,C> f);
		public Either<A,B> MapL (Action<A> f) {
			return MapL (f.ToFunc ());
		}
		public Either<A,B> MapL (Action f) {
			return MapL (f.ToFunc<A> ());
		}

		public abstract Either<B,A> Swap ();

		public Either<C,D> Both<C,D> (Func<A,C> L, Func<B,D> R) {
			return MapL (L).FMap (R);
		}

		public Either<C,B> Both<C> (Func<A,C> L, Action<B> R) {
			return MapL (L).FMap (R);
		}

		public Either<A,D> Both<D> (Action<A> L, Func<B,D> R) {
			return MapL (L).FMap (R);
		}

		public Either<A,B> Both (Action<A> L, Action<B> R) {
			return MapL (L).FMap (R);
		}

		public Either<A,B> Pure (B value) {
			return new Right<A,B> (value);
		}

		public abstract A left { get;}
		public abstract B right { get;}

		public abstract bool isRight { get;}
		public abstract bool isLeft { get;}

		Monad<C> Monad<B>.Bind<C> (Func<B, Monad<C>> f)
		{
			if (isRight)
				return f (right);
			else if (isLeft)
				return new Left<A,C> (left);
			else
				return new Neither<A,C> ();
		}

		Functor<B> Applicative<B>.Pure (B value)
		{
			return Pure (value);
		}

		Functor<C> Functor<B>.FMap<C> (Func<B, C> f)
		{
			return FMap (f);
		}

		public Functor<B> XMap (Func<Exception, Exception> fx)
		{
			throw new NotImplementedException ();
		}

		public abstract Either<A,B> IfFailed (Action f);
		public abstract Either<A,B> IfFailed (Action<A> f);

	}

	public class Right<A,B> : Either<A,B> {
		
		B val;

		public Right (B value) {
			this.val = value;
		}

		public override Either<A, C> Bind<C> (Func<B, Either<A, C>> f)
		{
			return f (val);
		}

		public override Either<C, B> BindL<C> (Func<A, Either<C, B>> f)
		{
			return new Right<C, B> (val);
		}

		public override Either<A, C> FMap<C> (Func<B, C> f)
		{
			return Fn.MaybeRight<A,C> (f (right));
		}

		public override Either<C, B> MapL<C> (Func<A, C> f)
		{
			return new Right<C, B> (val);
		}

		public override Either<B, A> Swap ()
		{
			return new Left<B,A> (val);
		}

		public override Either<A, B> IfFailed (Action f)
		{
			return this;
		}

		public override Either<A, B> IfFailed (Action<A> f)
		{
			return this;
		}

		public override A left {
			get {
				return default (A);
			}
		}

		public override B right {
			get {
				return val;
			}
		}

		public override bool isRight {
			get {
				return true;
			}
		}

		public override bool isLeft {
			get {
				return false;
			}
		}
	}

	public class Left<A,B> : Either<A,B> {

		A val;

		public Left (A value) {
			val = value;
		}

		public override Either<A, C> Bind<C> (Func<B, Either<A, C>> f)
		{;
			return new Left<A,C> (val);
		}

		public override Either<C, B> BindL<C> (Func<A, Either<C, B>> f)
		{
			return f (val);
		}

		public override Either<A, C> FMap<C> (Func<B, C> f)
		{
			return new Left<A,C> (val);
		}

		public override Either<C, B> MapL<C> (Func<A, C> f)
		{
			return Fn.MaybeLeft<C,B> (f (val));
		}

		public override Either<B, A> Swap ()
		{
			return new Right<B, A> (val);
		}

		public override Either<A, B> IfFailed (Action f)
		{
			f ();
			return this;
		}
		
		public override Either<A, B> IfFailed (Action<A> f)
		{
			f (val);
			return this;
		}

		public override A left {
			get {
				return val;
			}
		}

		public override B right {
			get {
				return default (B);
			}
		}

		public override bool isRight {
			get {
				return false; 
			}
		}

		public override bool isLeft {
			get {
				return true;
			}
		}
	}

	public class Neither<A,B> : Either<A,B> {

		public Neither() {}

		public override Either<A, C> Bind<C> (Func<B, Either<A, C>> f)
		{
			return new Neither<A,C> ();
		}

		public override Either<C, B> BindL<C> (Func<A, Either<C, B>> f)
		{
			return new Neither<C, B> ();
		}

		public override Either<A, C> FMap<C> (Func<B, C> f)
		{
			return new Neither<A,C> ();
		}

		public override Either<C, B> MapL<C> (Func<A, C> f)
		{
			return new Neither<C, B> ();
		}

		public override Either<B, A> Swap ()
		{
			return new Neither<B, A> ();
		}

		public override A left {
			get {
				return default (A);
			}
		}

		public override B right {
			get {
				return default (B);
			}
		}

		public override bool isRight {
			get {
				return false;
			}
		}

		public override bool isLeft {
			get {
				return false;
			}
		}

		public override Either<A, B> IfFailed (Action f)
		{
			f ();
			return this;
		}
		
		public override Either<A, B> IfFailed (Action<A> f)
		{
			return this;
		}

	}

	public static partial class Fn {

		public static Either<A,B> Right<A,B> (B right) {
			return new Right<A, B> (right);
		}

		public static Either<A,B> Left<A,B> (A left) {
			return new Left<A, B> (left);
		}

		public static Either<A,B> Either<A,B> (A left, B right) {

			var condR = right != null;
			var condL = left != null;

			
			if (condR)
				return new Right<A,B> (right);
			
			else if (condL) {
				return new Left<A,B> (left);
			}
			else {
				return new Neither<A,B> ();
			}
		}

		public static Either<A,string> Either<A> (A left, string right) {

			var condR = right != null && right != "";
			var condL = left != null;

			if (condR)
				return new Right<A,string> (right);

			else if (condL) {
				return new Left<A,string> (left);
			}

			else {
				return new Neither<A,string> ();
			}
		}

		public static Either<A,B> MaybeRight<A,B> (B value) {
			
			if (value != null)
				return new Right<A,B> (value);
			
			else {
				return new Neither<A, B> ();
			}
		}

		public static Either<A,B> MaybeLeft<A,B> (A value) {
			
			if (value != null)
				return new Left<A,B> (value);
			
			else {
				return new Neither<A, B> ();
			}
		}

		public static Maybe<A> ToMaybe<A> (this Either<A,A> either) {

			if (either.isRight)
				return Fn.Maybe (either.right);

			else if (either.isLeft)
				return Fn.Maybe (either.left);

			else
				return Fn.Nothing<A> ();
		}
	}
}