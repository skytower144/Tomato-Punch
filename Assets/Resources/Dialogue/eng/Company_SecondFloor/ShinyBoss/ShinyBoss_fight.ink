VAR cut = "/cut"
VAR ignore = "/ignore"
VAR br = "<br>"

#cutsetcamera:playerX@65.2

#cutteleport:Company_SecondFloor_worker_A@-115.51@60.06
#cutteleport:Company_SecondFloor_worker_B@-123@61.65
#cutteleport:Company_SecondFloor_worker_C@-117.54@62.96
#cutteleport:Company_SecondFloor_worker_D@-116.34@61.7
#cutteleport:Company_SecondFloor_worker_E@-125.88@62.24
#cutteleport:Company_SecondFloor_worker_F@-123.09@63.89
#cutteleport:Company_SecondFloor_worker_G@-125.1@61.52
#cutteleport:Company_SecondFloor_worker_H@-119.53@60.09
#cutteleport:Company_SecondFloor_worker_I@-115.17@61.98
#cutteleport:Company_SecondFloor_worker_J@-119.69@63.04

#cutplay:Company_SecondFloor_worker_A@employee_scattered_0
#cutplay:Company_SecondFloor_worker_B@employee_scattered_1
#cutplay:Company_SecondFloor_worker_C@employee_scattered_2
#cutplay:Company_SecondFloor_worker_D@employee_scattered_3
#cutplay:Company_SecondFloor_worker_E@employee_scattered_4
#cutplay:Company_SecondFloor_worker_F@employee_scattered_5
#cutplay:Company_SecondFloor_worker_G@employee_scattered_2
#cutplay:Company_SecondFloor_worker_H@employee_scattered_4
#cutplay:Company_SecondFloor_worker_I@employee_scattered_4
#cutplay:Company_SecondFloor_worker_J@employee_scattered_3

#cutteleport:!company_desk_0@-127.39@64.05
#cutteleport:!company_desk_1@-126.09@63.45
#cutteleport:!company_desk_2@-126.69@60.85
#cutteleport:!company_desk_3@-124.69@60.85
#cutteleport:!company_desk_4@-124.19@60.15
#cutteleport:!company_desk_5@-117.38@63.45
#cutteleport:!company_desk_6@-114.95@62.95
#cutteleport:!company_desk_7@-117.21@61.15
#cutteleport:!company_desk_8@-115.44@61.35
#cutteleport:!company_desk_9@-115.91@60.25
#cutrotate:!company_desk_0@-71.4
#cutrotate:!company_desk_1@120.9
#cutrotate:!company_desk_2@-39.9
#cutrotate:!company_desk_3@48.6
#cutrotate:!company_desk_4@-58.3
#cutrotate:!company_desk_5@97.9
#cutrotate:!company_desk_6@50.9
#cutrotate:!company_desk_7@-35.7
#cutrotate:!company_desk_8@61.5
#cutrotate:!company_desk_9@-95.3

#cutteleport:!company_chair_0@-127.49@62.955
#cutteleport:!company_chair_1@-125.09@62.855
#cutteleport:!company_chair_2@-125.29@59.755
#cutteleport:!company_chair_3@-126.29@62.355
#cutteleport:!company_chair_4@-126.29@60.555
#cutteleport:!company_chair_5@-117.41@62.355
#cutteleport:!company_chair_6@-114.65@63.255
#cutteleport:!company_chair_7@-114.32@60.355
#cutteleport:!company_chair_8@-117.91@61.355
#cutteleport:!company_chair_9@-115.22@61.855
#cutrotate:!company_chair_0@82.5
#cutrotate:!company_chair_1@-72.9
#cutrotate:!company_chair_2@90.8
#cutrotate:!company_chair_3@67.3
#cutrotate:!company_chair_4@40.8
#cutrotate:!company_chair_5@-64.8
#cutrotate:!company_chair_6@26.3
#cutrotate:!company_chair_7@-44.7
#cutrotate:!company_chair_8@73.3
#cutrotate:!company_chair_9@45.6
#cutwait:1.2
{cut}

#portrait:Boss_neutral
Congratulations.

By assaulting my employees,{br}you've officially become a criminal.

Such a pity that not a single employee meets my standards.

#cutlayer:Company_SecondFloor_Friend@Foreground
#cutplay:Company_SecondFloor_ShinyBoss@ShinyBoss_jump@wait
{cut}
I'll show you what real work ethic looks like.

#battletarget:Company_SecondFloor_ShinyBoss
#continuetalk:Company_SecondFloor_ShinyBoss@true
{ignore}