# SeaCombat

This project is a copy for this naval combat game.
https://www.youtube.com/watch?v=q4tdhVyZ_fo

It's sea combat with following rules:
- Ship can move forward, left, right or none, 4 times in a round.
- At every move, ship can shoot once or twice on left and right side. It can fire 2 shots at same time, 1 on every side Shooting happens before moving.
- All ships move and shoot at the same time. First everyone shoots, then moves. If only 1 ship move or shoot, others have to wait for animation to finish.
- The sea can apply effects to ships, like wind and whirlwinds. These activate after every shoot and after every move.

I implemented grid, turns, timers, basic movement, rotation and menus.

Basic scene is in "battle". The scene is visualy empty because grid and ship are generated at runtime.

Controls:
Press any of buttons once or multiple times to set moves:
1x: left
2x: forward
3x: right
4x: none

Check the boxes to shoot. There are no visuals, but the "wait for animation" counts.


Missing are the rules for shooting, sea actions, and enemies.
Moving has bugs.

The ui is lacking a bit.



Overall it was an interesting project, because I wanted to do it with a system that would allow me to easily add new rules without changing the Ui settings or the logic for combat.
