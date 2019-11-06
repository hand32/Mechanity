namespace roguelike
{
    public class boolOneTwo
    {
        public bool one;
        public bool two;

        public bool HasCollision()
        {
            return one || two;
        }
        public bool BothCollision()
        {
            return one && two;
        }
        public void Reset()
        {
            one = two = false;
        }
    }
    public class CharacterCollisionState2D
    {
        public boolOneTwo right = new boolOneTwo();
        public boolOneTwo left = new boolOneTwo();
        public boolOneTwo up = new boolOneTwo();
        public boolOneTwo down = new boolOneTwo();
        public bool rightUp;
        public bool rightDown;
        public bool leftDown;
        public bool leftUp;


        public bool HasParallelCollision()
        {
            return down.HasCollision() || right.HasCollision() || left.HasCollision() || up.HasCollision();
        }
        public bool HasDiagonalCollision()
        {
            return rightUp || rightDown || leftDown || leftUp;
        }

        public void Reset()
        {
            right.Reset();
            left.Reset();
            up.Reset();
            down.Reset();
            rightUp = rightDown = leftDown = leftUp = false;
        }

        public int ParallelCollisionCount()
        {
            int cnt = 0;

            if (down.BothCollision())
                cnt += 1;
            if (right.BothCollision())
                cnt += 1;
            if (left.BothCollision())
                cnt += 1;
            if (up.BothCollision())
                cnt += 1;

            return cnt;
        }


        public override string ToString()
        {
            return string.Format("[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}",
                                 right, left, up, down);
        }
    }
}