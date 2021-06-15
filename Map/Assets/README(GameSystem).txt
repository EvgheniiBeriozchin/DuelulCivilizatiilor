This is a presentation of the structure of the game. 

1.The game starts by choosing of the team's capital
The team simply presses the right-click on the territory they want to be their capital
2.Each team chooses/inputs the name of their team or this particular match //May be changed
The name is written in the InputField
3.(FROM ROUND 2 ONLY) Each team receives additional armies on their territories. The number of armies is equal to the (biggest) number of connected territories of this team.
The armies are distributed randomly throughout the territories.
4.All the teams answer trivia questions
//Right now it is done off the game
5.Additional points from the correctly answered trivia questions are added.
//If the questions will  be integrated in the game, this will be calculated automatically
6.Each team has the opportunity to attack.
In odd-numbered rounds they do it in ascending order, in even-numbered round - descending.
The attack is executed by pressing the Button attack, then choosing the hex(which belongs to the active team) which attack. 
After that the team chooses an adjancent hex, which will be attacked. After a short pause the result of the attack will be shown. 
If the territory is captured, the attacked hex is transfered to the winning team, all the armies from the attacking hex, but one, will be set in the newly captured hex. The attacking hex remains with ONE army only.
If the territory is not captured, the attacking hex loses all armies, but one. The attacked hex, or "defending hex", remains intact.
Each team can do as many attacks as the want, as long as they have territories which are able to attack. 
A hex is able to attack ONLY if it has TWO or MORE armies.
When the team finishes its attacks, the Button EndAttacks is pressed and the control passes to the next team.
After all teams attack, the first round is finished.

In rounds 2 - 8(TOTAL NUMBER OF ROUNDS), only steps 3-6 are executed.

After all the rounds the winner is identified by the number of WINNING POINTS it has.
WINNING POINTS = (2 * NUMBER OF CAPITAL TERRITORIES) + NUMBER OF OTHER TERRITORIES

ALGORITHM FOR CALCULATING THE ATTACKING AND DEFENDING POINTS:
ATT/DEF POINTS = NR. ARMIES IN THE CURRENT HEX + ADDITIONAL POINTS(from Trivia) + Random number from 1 to 6.
If the defending territory is neutral the formula is simplified to:
DEF POINTS = 2 + Random number from 1 to 6.

END OF THE DESCRIPTION OF THE GAME.

Here is presented a chain of methods, through which the GameSystem Script travels in order to satisfy the above algortihm.
Start(1) + Update(1) + Update(2) + GetInput(2) + (Update(5) + GetInput(5) + Update(6)) * Number of Rounds.
