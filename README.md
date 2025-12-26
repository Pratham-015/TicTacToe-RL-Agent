# TicTacToe-RL-Agent
## Made by Pratham Sharma - 250825

**This is my submission to 2nd assignment to Beat-The-Bot, Winter Project of GameDevClub IITK.**

Bonus : Video, BotVsBot(Only in Unity?)
## Index
- [How The Bot Learns](#-how-the-bot-learns)
- [Reward Curves](#-reward-curves)
- [Win Condition Logic](#-win-condition-logic)
- [Reflection Questions](#-reflection-questions)
- [Q table Reflection](#-q-table-reflection)
  (TL;DR : Due to time constraints, Q-table part used AI code and was incomplete but works fine.)
- [GameMode Change In Unity](#-gamemode-change-in-unity)

## How the Bot Learns
#### ML Agent
Initially, the bot learns through standard reward system introduced in week 1 (i.e. using MLAgents package). This happens by giving rewards based on its performance.
**Initial Reward Structure:**
- Win : 1
- Draw : 0.75
- Lose : 0
- Valid : 0.05
- Invalid : - 3.0

The agent initially does a lot of invalid moves due to randomness. However, it slowly stabilizes. But, it still does a lot of invalid moves.

So, the code and mechanism is updated to prevent the agent from making invalid moves.
**New Reward Structure:**
- Win : 1
- Lose : -1
- Draw :0.2
Also, previously there was only 1 agent alternating player. Now, 2 agents alternate between turns

This is the current ML-Agent model for the GameMode.
It mostly draws against itself but loses most of the times against an human.

#### Q-Table
In an attempt to improve, the Q-table method of training was also used.
However, due to lack of time, most of the code was written by AI (it still works).
The Q-table works by setting a Q-value for every state and action possible (19683 states and 9 actions).
In the training process, it constantly updates Q-values by using this formula.
![[Pasted image 20251226235915.png]]
Here, alpha= learning rate,
r= immediate reward,
gamma = discount factor
After the training is over, you can play against it.
The reward system remains similar except that Draw Reward goes from +0.2 -> 0.05
However, the agent still loses sometimes.

## Reward Curves
**Reward** (Goes form -370 to -60) (Tic_v1)
![[Pasted image 20251226234121.png]]
**Reward** (Tic_v2)
![[Pasted image 20251226234659.png]]
## Win Condition Logic
The function used (WinCheck in GameManager.cs) checks every time a new turn is made. It takes in arguments (row,col,currentPlayer). 
Here, row and col decide the tile of the latest move. currentPlayer tells what is the tile status. (Red-X->1) ; (Blue-O->0)
It then checks the row and column of that tile. If the tile lies on a diagonal, it checks for that as well.
Returns 1 if won and 0 if nothing found.

## Reflection Questions
-  **What was the first major flaw in your agent’s behavior?**
	The first flaw was that it kept on making invalid moves. Even after adding penalty for invalid and rewards for valid moves, it still made invalid moves as seen from reward curves.
- **What was the primary cause of this flaw?**
  There was no way for the Agent to prevent invalid moves. Expecting it to learn that is a mistake when we also want it to be random and explore. So, we had to code it so that it cannot act on a played tile.
- **Choose one and justify:**
	**Reward design**
	**Observation space**
	**Exploration strategy**
  Reward Design is very important as it can easily break an agent if it is incorrect. An agent might act in a different way than expected to farm rewards.
- **What exact change did you make to fix it?**
  I ensured that every move of the agent is valid by masking the actions where the board is not empty.
- **How did the reward curve change after the fix?**
  The reward curve ended up getting positive and slowly increasing. The previous reward curve was negative due to Invalid Penalty.
- **Describe differences in:**
	**Stability**
	The agent is more stable as in the end, it ended up converging to a single board.
	**Learning speed**
	Learning speed was high earlier and kept getting lower after many episodes. Previously, the learning speed felt chaotic.
	**Final performance**
	There is a good improvement in performance as the previous agent still failed to act while the agent is able to perform well enough.
- **What does this tell you about instability in RL training?**
  This shows that RL is very sensitive to reward design, but also to the game environment itself. A small error in logic could lead to the doom of the agent and lots of wasted time.

## Q Table Reflection
After this, I decided to change from implement from ML Agents to Q-Table. But, I was busy on day 3, so I had to complete this in 2 days. I rushed and blindly copy-pasted code from AI. While the new model still works ok, there were faults like no intermediate reward which would make the agent better. Anyway, the build contains the Q-table version, but in unity you could manually change the gamemode.

## GameMode Change in Unity
In game manager object, there are 2 fields in the script:
- Game Mode:
	- Self Play (MLAgent)
	- BotVsBot (MLAgent)
	- HumanVsBot (Both)
	- QTraining (QTable)
- ai:
	- MLAgent
	- QTable
These modes could have been accessed through a menu, but I failed to implement it. But, you can still play it in unity and play against a bot with both ai (mlagent and q-table).
