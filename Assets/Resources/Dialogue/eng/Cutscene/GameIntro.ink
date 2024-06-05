VAR cut = "/cut"
VAR ignore = "/ignore"
VAR br = "<br>"

#teleport:player@-70.44@-2.12
#teleport:TomatoHouse_Friend@-66.56@-0.99
#uicanvaslayer:front_ui@3
{ignore}

#cutturn:player@RU-0
#cutplay:TomatoHouse_Friend@Friend_sit_sofa
#cutplay:!TomatoHouse_tv@TomatoHouse_tv_flicker
#cutfadein:1@1.5@4@wait
#cutwait:0.8
{cut}
#hideportrait:_
"...this is some intense battle right here!"

#movedialoguebox:-220@533
#movecursor:627@-125
#setboxalpha:0
#textcolor:white
{ignore}

#cutspawn:0
#cutplay:!BigSmack@GameIntro_BigSmack_getup
{cut}
Will he be able to get up?

...And the deadly battle continues!

#cutplay:!Opponent@GameIntro_opponent_rush
#cutplay:!BigSmack@GameIntro_BigSmack_guard
{cut}
Once again with the rush!{br}This isn't looking too good!

#cutplay:!Opponent@GameIntro_opponent_ko
#cutplay:!BigSmack@GameIntro_BigSmack_smack
#cutplay:!VictoryFlash@GameIntro_VictoryFlash
{cut}
AND THERE'S THE SIGNATURE MOVE!{br}THE BIG SMACK!

#cutplay:!BigSmack@GameIntro_BigSmack_recover
#cutplay:!Referee@GameIntro_referee_judge
{cut}
That's the cleanest hit I've ever witnessed!{br}Will he get up?

#cutplay:!Referee@GameIntro_referee_ko@wait
#cutplay:!BigSmack@GameIntro_BigSmack_victory
#cutplay:!Crowd@GameIntro_crowd_cheer
#cutplay:!VictoryFlash@GameIntro_VictoryFlash_repeat
{cut}
And that's a KO!{br}Another magnificent victory for Big Smack!

#cutdestroy:0
#cutwait:0.8
{cut}

#resetdialoguebox:_
#portrait:Friend_neutral
You sure love watching this ancient relic.

#portrait:Tomato_neutral
Love it?

#hideportrait:_
#uicanvaslayer:front_ui@3
#movedialoguebox:-220@533
#movecursor:627@-125
#setboxalpha:0
#textcolor:white
{ignore}

#cutfadeout:0.6@0@4@wait
#cutimage:0@true
#cutfadein:0.6@0@4@wait
{cut}
It's perfection.

#cutimage:0@false
{cut}

#resetdialoguebox:_
{ignore}

insert sound: knock knock knock

#cutwait:1
#cutturn:player@RD-0
{cut}
#hideportrait:_
I know you're in there!{br}Are you out of your mind?

#cutplay:TomatoHouse_Friend@Friend_lookR_sofa
{cut}
#portrait:Friend_neutral
Relax.

#cutplay:TomatoHouse_Friend@Friend_sit_sofa
#cutturn:player@RIGHT-0
{cut}
#portrait:Friend_bottle
I'm just taking my coffee break.

#cutturn:player@RD-0
{cut}
#hideportrait:_
Boss is mad at you.

#cutplay:TomatoHouse_Friend@Friend_lookD_sofa
{cut}
Which means BIG trouble.

#cutwait:1
#cutplay:TomatoHouse_Friend@Friend_worried_sofa
{cut}
#portrait:Friend_neutral
Dang.

#cutturn:player@RIGHT-0
#cutplay:TomatoHouse_Friend@Friend_drink_sofa@wait
{cut}
Sigh.

You better start looking for a real job.

...or you'll end up like me.

#cutplay:TomatoHouse_Friend@Friend_Idle
{cut}
Guess I'm off to work.

#cutturn:player@RIGHT-1,RD-1
#cutmove:TomatoHouse_Friend@-68.68~-0.99,-68.68~-3.8,-66.73~-3.8,-66.73~-4.6@3@true@wait
#cutplay:!TomatoHouse_tv@TomatoHouse_tv_default
{cut}

#addkeyevent:Scolded_Friend_CompanySecondFloor
#setactive:TomatoHouse_Friend@false
{ignore}