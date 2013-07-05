//id посылки на acm.timus.ru - 4899107 (пройден только 41 тест)

using System;


public class Edge
{
    public int a;
    public Edge next;
};

public struct Part
{
    public int w;
    public int d;
    public int h;
    public String s;
};

public class Timus
{
    public static int []labeling_corners(string s) {
	    String temp = "ROYGBV";
        int []t = new int[8];
	    for (int i=0; i<8; i++) {
	    	t[i] = 0;
	    }
        t[0] |= (1 << temp.IndexOf(s[0])) | (1 << temp.IndexOf(s[2])) | (1 << temp.IndexOf(s[5]));
        t[1] |= (1 << temp.IndexOf(s[0])) | (1 << temp.IndexOf(s[3])) | (1 << temp.IndexOf(s[5]));
        t[2] |= (1 << temp.IndexOf(s[0])) | (1 << temp.IndexOf(s[2])) | (1 << temp.IndexOf(s[4]));
        t[3] |= (1 << temp.IndexOf(s[0])) | (1 << temp.IndexOf(s[3])) | (1 << temp.IndexOf(s[4]));
        t[4] |= (1 << temp.IndexOf(s[1])) | (1 << temp.IndexOf(s[2])) | (1 << temp.IndexOf(s[5]));
        t[5] |= (1 << temp.IndexOf(s[1])) | (1 << temp.IndexOf(s[3])) | (1 << temp.IndexOf(s[5]));
        t[6] |= (1 << temp.IndexOf(s[1])) | (1 << temp.IndexOf(s[2])) | (1 << temp.IndexOf(s[4]));
        t[7] |= (1 << temp.IndexOf(s[1])) | (1 << temp.IndexOf(s[3])) | (1 << temp.IndexOf(s[4]));
	    return t;
    }
    public static int []labeling_edges(string s) {
    	String temp = "ROYGBV";
    	int []t = new int[12];
    	for (int i=0; i<12; i++) {
    		t[i] = 0;
    	}
        t[0] |= (1 << temp.IndexOf(s[0])) | (1 << temp.IndexOf(s[2]));
        t[1] |= (1 << temp.IndexOf(s[0])) | (1 << temp.IndexOf(s[3]));
        t[2] |= (1 << temp.IndexOf(s[1])) | (1 << temp.IndexOf(s[2]));
        t[3] |= (1 << temp.IndexOf(s[1])) | (1 << temp.IndexOf(s[3]));
        t[4] |= (1 << temp.IndexOf(s[0])) | (1 << temp.IndexOf(s[4]));
        t[5] |= (1 << temp.IndexOf(s[0])) | (1 << temp.IndexOf(s[5]));
        t[6] |= (1 << temp.IndexOf(s[1])) | (1 << temp.IndexOf(s[4]));
        t[7] |= (1 << temp.IndexOf(s[1])) | (1 << temp.IndexOf(s[5]));
        t[8] |= (1 << temp.IndexOf(s[2])) | (1 << temp.IndexOf(s[4]));
        t[9] |= (1 << temp.IndexOf(s[2])) | (1 << temp.IndexOf(s[5]));
        t[10] |= (1 << temp.IndexOf(s[3])) | (1 << temp.IndexOf(s[4]));
        t[11] |= (1 << temp.IndexOf(s[3])) | (1 << temp.IndexOf(s[5]));
    	return t;
    }
    public static int binary(String s) {
    	String temp = "ROYGBV";
    	int t = 0;
    	for (int i=0; i<6; i++) {
    		if (s[i] != '.') {
                t |= (1 << temp.IndexOf(s[i]));
    		}
    	}
    	return t;
    }
    public static void orientation(int mask, String bs, String s, ref int front, ref int bottom, ref int left) {
    	String color = "ROYGBV";
    	if ((mask&(1<<color.IndexOf(bs[0]))) != 0)
        {
    		front = s.IndexOf(bs[0]);
    	}
        else if ((mask&(1<<color.IndexOf(bs[1]))) != 0)
        {
    		front = (s.IndexOf(bs[1])&1) != 0 ? s.IndexOf(bs[1])-1 : s.IndexOf(bs[1])+1;
    	}
        else
        {
    		front = -1;
    	}
        if ((mask & (1 << color.IndexOf(bs[2]))) != 0)
        {
    		bottom = s.IndexOf(bs[2]);
        }
        else if ((mask & (1 << color.IndexOf(bs[3]))) != 0)
        {
            bottom = (s.IndexOf(bs[3]) & 1) != 0 ? s.IndexOf(bs[3]) - 1 : s.IndexOf(bs[3]) + 1;
    	}
        else
        {
    		bottom = -1;
    	}
        if ((mask & (1 << color.IndexOf(bs[4]))) != 0)
        {
            left = s.IndexOf(bs[4]);
        }
        else if ((mask & (1 << color.IndexOf(bs[5]))) != 0)
        {
            left = (s.IndexOf(bs[5]) & 1) != 0 ? s.IndexOf(bs[5]) - 1 : s.IndexOf(bs[5]) + 1;
    	}
        else
        {
    		left = -1;
    	}
    	if (front == -1 && bottom >-1 && left > -1) {
    		switch (bottom) {
    			case 0:				
    				if (left < 4) {
    					front = 4+(left&1);
    				} else {
    					front = 3-(left&1);
    				}
    				break;
    			case 1:
    				if (left < 4) {
    					front = 5-(left&1);
    				} else {
    					front = 2+(left&1);
    				}
    				break;
    			case 2:
    				if (left < 2) {
    					front = 5-(left&1);
    				} else {
    					front = left&1;
    				}
    				break;
    			case 3:
    				if (left < 2) {
    					front = 4+(left&1);
    				} else {
    					front = 1-(left&1);
    				}
    				break;
    			case 4:
    				if (left < 2) {
    					front = 2+(left&1);
    				} else {
    					front = 1-(left&1);
    				}
    				break;
    			default:
    				if (left < 2) {
    					front = 3-(left&1);
    				} else {
    					front = left&1;
    				}
                    break;
    		}
    	}
    	if (bottom == -1 && front > -1 && left > -1) {
    		switch (front) {
    			case 0:
    				if (left < 4) {
    					bottom = 5-(left&1);
    				} else {
    					bottom = 2+(left&1);
    				}
    				break;
    			case 1:
    				if (left < 4) {
    					bottom = 4+(left&1);
    				} else {
    					bottom = 3-(left&1);
    				}
    				break;
    			case 2:
    				if (left < 2) {
    					bottom = 4+(left&1);
    				} else {
    					bottom = 1-(left&1);
    				}
    				break;
    			case 3:
    				if (left < 2) {
    					bottom = 5-(left&1);
    				} else {
    					bottom = left&1;
    				}
    				break;
    			case 4:
    				if (left < 2) {
    					bottom = 3-(left&1);
    				} else {
    					bottom = left&1;
    				}
    				break;
    			default:
    				if (left < 2) {
    					bottom = 2+(left&1);
    				} else {
    					bottom = 1-(left&1);
    				}
                    break;
    		}
    	}
    	if (left == -1 && front > -1 && bottom > -1) {
    		switch (front) {
    			case 0:
    				if (bottom < 4) {
    					left = 4+(bottom&1);
    				} else {
    					left = 3-(bottom&1);
    				}
    				break;
    			case 1:
    				if (bottom < 4) {
    					left = 5-(bottom&1);
    				} else {
    					left = 2+(bottom&1);
    				}
    				break;
    			case 2:
    				if (bottom < 2) {
    					left = 5-(bottom&1);
    				} else {
    					left = bottom&1;
    				}
    				break;
    			case 3:
    				if (bottom < 2) {
    					left = 4+(bottom&1);
    				} else {
    					left = 1-(bottom&1);
    				}
    				break;
    			case 4:
    				if (bottom < 2) {
    					left = 2+(bottom&1);
    				} else {
    					left = 1-(bottom&1);
    				}
    				break;
    			default:
    				if (bottom < 2) {
    					left = 3-(bottom&1);
    				} else {
    					left = bottom&1;
    				}
                    break;
    		}
    	}
    }
    public static int []find(int mask, String bs, String s, int w, int d, int h) {
    	int []temp = new int[3];
    	int front = -1,bottom = -1,left = -1;
    	orientation(mask,bs,s,ref front,ref bottom,ref left);
    	if (front == 0 || front == 1) {
    		temp[0] = w;
    		if (bottom == 2 || bottom == 3) {
    			temp[1] = d; temp[2] = h;
    		} else {
    			temp[1] = h; temp[2] = d;
    		}
    	} else if (front == 2 || front == 3) {
    		temp[0] = d;
    		if (bottom == 0 || bottom == 1) {
    			temp[1] = w; temp[2] = h;
    		} else {
    			temp[1] = h; temp[2] = w;
    		}
    	} else {
    		temp[0] = h;
    		if (bottom == 0 || bottom == 1) {
    			temp[1] = w; temp[2] = d;
    		} else {
    			temp[1] = d; temp[2] = w;
    		}
    	}
    	return temp;
    }
    public static bool compare(int a, int b)
    {
        if (b == (a & b))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private static void Main()
    {
	    string rule_color = "ROYGBV";
        
//        System.IO.StreamReader file = new System.IO.StreamReader("C:/Users/user/Desktop/diff.txt");
//        String[] tempstr = file.ReadLine().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        String []tempstr = Console.ReadLine().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        
        int base_w = Convert.ToInt32(tempstr[0]);
        int base_d = Convert.ToInt32(tempstr[1]);
        int base_h = Convert.ToInt32(tempstr[2]);
        String bs = tempstr[3];

//        tempstr = file.ReadLine().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        tempstr = Console.ReadLine().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        int n = Convert.ToInt32(tempstr[0]);
    	int []corners = labeling_corners(bs); int []edges = labeling_edges(bs);
    	Part []all = new Part[n];
        Edge x = new Edge(); Edge y = new Edge(); Edge z = new Edge();
        x.a = 0; y.a = 0; z.a = 0;
	    x.next = new Edge(); y.next = new Edge(); z.next = new Edge();
	    Edge x_last = x.next; Edge y_last = y.next; Edge z_last = z.next;
	    x_last.a = 0; y_last.a = 0; z_last.a = 0;
	    x_last.next = null; y_last.next = null; z_last.next = null;
	    Edge x_cur = x; Edge y_cur = y; Edge z_cur = z;
	    int x_amount = 2, y_amount = 2, z_amount = 2;
	    for (int i=0; i<n; i++) {

//            tempstr = file.ReadLine().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            tempstr = Console.ReadLine().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            all[i].w = Convert.ToInt32(tempstr[0]);
            all[i].d = Convert.ToInt32(tempstr[1]);
            all[i].h = Convert.ToInt32(tempstr[2]);
            all[i].s = tempstr[3];
    		int temps = binary(all[i].s);
		    bool was = false;
		    for (int j=0; j<8; j++) {
			    if (compare(temps,corners[j])) {
				    if (j == 2) {
		    			int []t = find(corners[2],bs,all[i].s,all[i].w,all[i].d,all[i].h);
		    			x.a = t[0]; y.a = t[1]; z.a = t[2];
			    	} else if (j == 5) {
				    	int []t = find(corners[5],bs,all[i].s,all[i].w,all[i].d,all[i].h);
    					x_last.a = t[0]; y_last.a = t[1]; z_last.a = t[2];
	    			}
		    		was = true;
			    }
		    }
		    if (!was) {
		    	if (compare(temps,edges[8])) {
		    		Edge temp = new Edge();
		    		int []t = find(edges[8],bs,all[i].s,all[i].w,all[i].d,all[i].h);
		    		temp.a = t[0];
		    		temp.next = x_last;
		    		x_cur.next = temp;
		    		x_cur = temp;
		    		x_amount++;
		    	} else if (compare(temps,edges[4])) {
		    		Edge temp = new Edge();
		    		int []t = find(edges[4],bs,all[i].s,all[i].w,all[i].d,all[i].h);
		    		temp.a = t[1];
		    		temp.next = y_last;
		    		y_cur.next = temp;
		    		y_cur = temp;
		    		y_amount++;
		    	} else if (compare(temps,edges[0])) {
		    		Edge temp = new Edge();
		    		int []t = find(edges[0],bs,all[i].s,all[i].w,all[i].d,all[i].h);
		    		temp.a = t[2];
		    		temp.next = z_last;
		    		z_cur.next = temp;
		    		z_cur = temp;
		    		z_amount++;
		    	}
		    }
	    }
    	int [,,]cube = new int [x_amount,y_amount,z_amount];
    	for (int i=0; i<x_amount; i++) {
    		for (int j=0; j<y_amount; j++) {
    			for (int k=0; k<z_amount; k++) {
    				cube[i,j,k] = 0;
    			}
    		}
    	}
    	String position = "FBDULR";
    	for (int i=0; i<n; i++) {
    		bool t = false;
    		int temp = binary(all[i].s),front = -1,bottom = -1,left = -1;
    		for (int j=0; j<8 && !t; j++) {
    			if (compare(temp,corners[j])) {
    				orientation(corners[j],bs,all[i].s,ref front,ref bottom,ref left);
                    Console.Write(position[front] + " " + position[bottom] + " ");
    				if (j < 4) {
    					Console.Write("0 ");
    				} else {
                        Console.Write(base_w - x_last.a + " ");
    				}
    				if ((j&1) == 0) {
                        Console.Write("0 ");
    				} else {
                        Console.Write(base_d - y_last.a + " ");
    				}
    				if ((j&3) < 2) {
                        Console.WriteLine(base_h - z_last.a);
    				} else {
                        Console.WriteLine("0");
    				}
    				t = true;
    			}
    		}
    		if (!t) {
    			for (int j=0; j<12 && !t; j++) {
    				if (compare(temp,edges[j])) {
    					orientation(edges[j],bs,all[i].s,ref front,ref bottom,ref left);
                        Console.Write(position[front] + " " + position[bottom] + " ");
    					int []cur = find(edges[j],bs,all[i].s,all[i].w,all[i].d,all[i].h);
    					int x_temp,y_temp,z_temp,cube_x,cube_y,cube_z;
    					if (j<4) {
    						switch (j) {
    							case 0:
    								cube_x = 0; cube_y = 0;
    								x_temp = 0; y_temp = 0;
    								break;
    							case 1:
    								cube_x = 0; cube_y = y_amount-1;
    								x_temp = 0; y_temp = base_d-y_last.a;
    								break;
    							case 2:
    								cube_x = x_amount-1; cube_y = 0;
    								x_temp = base_w-x_last.a; y_temp = 0;
    								break;
    							default:
    								cube_x = x_amount-1; cube_y = y_amount-1;
    								x_temp = base_w-x_last.a; y_temp = base_d-y_last.a;
                                    break;
    						}
    						z_cur = z.next;
    						z_temp = z.a;
    						cube_z = 1;
    						while (z_cur.next != null) {
    							if (cur[2] == z_cur.a && cube[cube_x,cube_y,cube_z] == 0) {
    								cube[cube_x,cube_y,cube_z] = 1;
    								break;
    							} else {
    								z_temp+=z_cur.a;
    								z_cur = z_cur.next;
    								cube_z++;
    							}
    						}
    						Console.WriteLine(x_temp + " " + y_temp + " " + z_temp);
    					} else if (j<8) {
    						switch (j) {
    							case 4:
    								x_temp = 0; z_temp = 0;
    								cube_x = 0; cube_z = 0;
    								break;
    							case 5:
    								x_temp = 0; z_temp = base_h-z_last.a;
    								cube_x = 0; cube_z = z_amount-1;
    								break;
    							case 6:
    								x_temp = base_w-x_last.a; z_temp = 0;
    								cube_x = x_amount-1; cube_z = 0;
    								break;
    							default:
    								x_temp = base_w-x_last.a; z_temp = base_h-z_last.a;
    								cube_x = x_amount-1; cube_z = z_amount-1;
                                    break;
    						}
    						y_cur = y.next;
    						y_temp = y.a;
    						cube_y = 1;
    						while (y_cur.next != null) {
    							if (cur[1] == y_cur.a && cube[cube_x,cube_y,cube_z] == 0 ) {
    								cube[cube_x,cube_y,cube_z] = 1;
    								break;
    							} else {
    								y_temp+=y_cur.a;
    								y_cur = y_cur.next;
    								cube_y++;
    							}
    						}
                            Console.WriteLine(x_temp + " " + y_temp + " " + z_temp);
    					} else {
    						switch (j) {
    							case 8:
    								y_temp = 0; z_temp = 0;
    								cube_y = 0; cube_z = 0;
    								break;
    							case 9:
    								y_temp = 0; z_temp = base_h-z_last.a;
    								cube_y = 0; cube_z = z_amount-1;
    								break;
    							case 10:
    								y_temp = base_d-y_last.a; z_temp = 0;
    								cube_y = y_amount-1; cube_z = 0;
    								break;
    							default:
    								y_temp = base_d-y_last.a; z_temp = base_h-z_last.a;
    								cube_y = y_amount-1; cube_z = z_amount-1;
                                    break;
    						}
    						x_cur = x.next;
    						x_temp = x.a;
    						cube_x = 1;
    						while (x_cur.next != null) {
    							if (cur[0] == x_cur.a && cube[cube_x,cube_y,cube_z] == 0) {
    								cube[cube_x,cube_y,cube_z] = 1;
    								break;
    							} else {
    								x_temp+=x_cur.a;
    								x_cur = x_cur.next;
    								cube_x++;
    							}
    						}
                            Console.WriteLine(x_temp + " " + y_temp + " " + z_temp);
    					}
    					t = true;
    				}
    			}
    			if (!t) {
    				for (int j=0; j<6 && !t; j++) {
    					if (compare(temp,1<<j)) {
    						int temp_x=x.a,temp_y=y.a,temp_z=z.a,temp_color,a,b,cube_x,cube_y,cube_z;
    						string temp_str = bs;
    						temp_color = temp_str.IndexOf(rule_color[j]);
    						if (temp_color<2) {
    							temp_str = all[i].s;
    							front = (temp_color&1) != 0 ? (temp_str.IndexOf(bs[1])/2)*2+(temp_str.IndexOf(bs[1])+1)%2 : temp_str.IndexOf(bs[0]);
    							if (front < 2) {
    								a = all[i].d; b = all[i].h;
    							} else if (front < 4) {
    								a = all[i].w; b = all[i].h;
    							} else {
    								a = all[i].w; b = all[i].d;
    							}
    							temp_x = (temp_color&1) != 0 ? base_w-x_last.a : 0;
    							cube_x = (temp_color&1) != 0 ? x_amount-1 : 0;
    							y_cur = y.next;
    							cube_y = 1;
    							bool t1 = false, t2 = false;
    							while (y_cur.next != null && t1 != true) {
    								if (y_cur.a == a) {
    									z_cur = z.next;
    									cube_z = 1;
    									temp_z = z.a;
    									while (z_cur.next != null && t2 != true) {
    										if (z_cur.a == b && cube[cube_x,cube_y,cube_z] == 0) {
    											if (front < 2) {
    												bottom = 2;
    											} else if (front < 4) {
    												bottom = 0;
    											} else {
    												bottom = 0;
    											}
    											cube[cube_x,cube_y,cube_z] = 1;
    											t2 = true;
    										}
    										if (!t2) {
    											temp_z+=z_cur.a;
    											z_cur = z_cur.next;
    											cube_z++;
    										}
    									}
    									if (t2) {
    										t1 = true;
    									}
    								}
    								if (y_cur.a == b && !t1) {
    									z_cur = z.next;
    									cube_z = 1;
    									temp_z = z.a;
    									while (z_cur.next != null && t2 != true) {
    										if (z_cur.a == a && cube[cube_x,cube_y,cube_z] == 0) {
    											if (front < 2) {
    												bottom = 4;
    											} else if (front < 4) {
    												bottom = 4;
    											} else {
    												bottom = 2;
    											}
    											cube[cube_x,cube_y,cube_z] = 1;
    											t2 = true;
    										}
    										if (!t2) {
    											temp_z+=z_cur.a;
    											z_cur = z_cur.next;
    											cube_z++;
    										}
    									}
    									if (t2) {
    										t1 = true;
    									}
    								}
    								if (!t1) {
    									temp_y+=y_cur.a;
    									y_cur = y_cur.next;
    									cube_y++;
    								}
    							}
    						} else if (temp_color<4) {
    							temp_str = all[i].s;
    							bottom = (temp_color&1) != 0 ? (temp_str.IndexOf(bs[3])/2)*2+(temp_str.IndexOf(bs[3])+1)%2 : temp_str.IndexOf(bs[2]);
    							if (bottom < 2) {
    								a = all[i].d; b = all[i].h;
    							} else if (bottom < 4) {
    								a = all[i].w; b = all[i].h;
    							} else {
    								a = all[i].w; b = all[i].d;
    							}
    							temp_y = (temp_color&1) != 0 ? base_d-y_last.a : 0;
    							cube_y = (temp_color&1) != 0 ? y_amount-1 : 0;
    							x_cur = x.next;
    							cube_x = 1;
    							bool t1 = false, t2 = false;
    							while (x_cur.next != null && t1 != true) {
    								if (x_cur.a == a) {
    									z_cur = z.next;
    									cube_z = 1;
    									temp_z = z.a;
    									while (z_cur.next != null && t2 != true) {
    										if (z_cur.a == b && cube[cube_x,cube_y,cube_z] == 0) {
    											if (bottom < 2) {
    												front = 2;
    											} else if (bottom < 4) {
    												front = 0;
    											} else {
    												front = 0;
    											}
    											cube[cube_x,cube_y,cube_z] = 1;
    											t2 = true;
    										}
    										if (!t2) {
    											temp_z+=z_cur.a;
    											z_cur = z_cur.next;
    											cube_z++;
    										}
    									}
    									if (t2) {
    										t1 = true;
    									}
    								}
    								if (x_cur.a == b && !t1) {
    									z_cur = z.next;
    									cube_z = 1;
    									temp_z = z.a;
    									while (z_cur.next != null && t2 != true) {
    										if (z_cur.a == a && cube[cube_x,cube_y,cube_z] == 0) {
    											if (bottom < 2) {
    												front = 4;
    											} else if (bottom < 4) {
    												front = 4;
    											} else {
    												front = 2;
    											}
    											cube[cube_x,cube_y,cube_z] = 1;
    											t2 = true;
    										}
    										if (!t2) {
    											temp_z+=z_cur.a;
    											z_cur = z_cur.next;
    											cube_z++;
    										}
    									}
    									if (t2) {
    										t1 = true;
    									}
    								}
    								if (!t1) {
    									temp_x+=x_cur.a;
    									x_cur = x_cur.next;
    									cube_x++;
    								}
    							}
    						} else {
    							temp_str = all[i].s;
    							left = (temp_color&1) != 0 ? (temp_str.IndexOf(bs[5])/2)*2+(temp_str.IndexOf(bs[5])+1)%2 : temp_str.IndexOf(bs[4]);
    							if (left < 2) {
    								a = all[i].d; b = all[i].h;
    							} else if (left < 4) {
    								a = all[i].w; b = all[i].h;
    							} else {
    								a = all[i].w; b = all[i].d;
    							}
    							temp_z = (temp_color&1) != 0 ? base_h-z_last.a : 0;
    							cube_z = (temp_color&1) != 0 ? z_amount-1 : 0;
    							x_cur = x.next;
    							cube_x = 1;
    							bool t1 = false, t2 = false;
    							while (x_cur.next != null && t1 != true) {
    								if (x_cur.a == a) {
    									y_cur = y.next;
    									cube_y = 1;
    									temp_y = y.a;
    									while (y_cur.next != null && t2 != true) {
    										if (y_cur.a == b && cube[cube_x,cube_y,cube_z] == 0) {
    											if (left < 2) {
    												front = 2;
    												bottom = 4;
    											} else if (left < 4) {
    												front = 0;
    												bottom = 4;
    											} else {
    												front = 0;
    												bottom = 2;
    											}
    											cube[cube_x,cube_y,cube_z] = 1;
    											t2 = true;
    										}
    										if (!t2) {
    											temp_y+=y_cur.a;
    											y_cur = y_cur.next;
    											cube_y++;
    										}
    									}
    									if (t2) {
    										t1 = true;
    									}
    								}
    								if (x_cur.a == b && !t1) {
    									y_cur = y.next;
    									cube_y = 1;
    									temp_y = y.a;
    									while (y_cur.next != null && t2 != true) {
    										if (y_cur.a == a && cube[cube_x,cube_y,cube_z] == 0) {
    											if (left < 2) {
    												front = 4;
    												bottom = 2;
    											} else if (left < 4) {
    												front = 4;
    												bottom = 0;
    											} else {
    												front = 2;
    												bottom = 0;
    											}
    											cube[cube_x,cube_y,cube_z] = 1;
    											t2 = true;
    										}
    										if (!t2) {
    											temp_y+=y_cur.a;
    											y_cur = y_cur.next;
    											cube_y++;
    										}
    									}
    									if (t2) {
    										t1 = true;
    									}
    								}
    								if (!t1) {
    									temp_x+=x_cur.a;
    									x_cur = x_cur.next;
    									cube_x++;
    								}
    							}
    						}
                            Console.WriteLine(position[front] + " " + position[bottom] + " " + temp_x + " " + temp_y + " " + temp_z);
    						t = true;
    					}
    				}
    				if (!t) {
    					int temp_x = x.a,temp_y = 0,temp_z = 0,cube_x = 1,cube_y,cube_z;
    					bool t1 = false, t2 = false, t3 = false;
    					x_cur = x.next;
    					while(x_cur.next != null && t1 != true) {
    						if (x_cur.a == all[i].w) {
    							y_cur = y.next;
    							cube_y = 1;
    							temp_y = y.a;
    							while (y_cur.next != null && t2 != true) {
    								if (y_cur.a == all[i].d) {
    									z_cur = z.next;
    									cube_z = 1;
    									temp_z = z.a;
    									while (z_cur.next != null && t3 != true) {
    										if (z_cur.a == all[i].h && cube[cube_x,cube_y,cube_z] == 0) {
    											cube[cube_x,cube_y,cube_z] = 1;
    											t3 = true;
    											front = 0; bottom = 2;
    										}
    										if (!t3) {
    											temp_z+= z_cur.a;
    											z_cur = z_cur.next;
    											cube_z++;
    										}
    									}
    									if (t3) {
    										t2 = true;
    									}
    								}
    								if (y_cur.a == all[i].h && !t2) {
    									z_cur = z.next;
    									cube_z = 1;
    									temp_z = z.a;
    									while (z_cur.next != null && t3 != true) {
    										if (z_cur.a == all[i].d && cube[cube_x,cube_y,cube_z] == 0) {
    											t3 = true;
    											cube[cube_x,cube_y,cube_z] = 1;
    											front = 0; bottom = 4;
    										}
    										if (!t3) {
    											temp_z+= z_cur.a;
    											z_cur = z_cur.next;
    											cube_z++;
    										}
    									}
    									if (t3) {
    										t2 = true;
    									}
    								}
    								if (!t2) {
    									temp_y+= y_cur.a;
    									y_cur = y_cur.next;
    									cube_y++;
    								} else {
    									t1 = true;
    								}
    							}
    						}
    						if (x_cur.a == all[i].d && !t1) {
    							y_cur = y.next;
    							cube_y = 1;
    							temp_y = y.a;
    							while (y_cur.next != null && t2 != true) {
    								if (y_cur.a == all[i].w) {
    									z_cur = z.next;
    									cube_z = 1;
    									temp_z = z.a;
    									while (z_cur.next != null && t3 != true) {
    										if (z_cur.a == all[i].h && cube[cube_x,cube_y,cube_z] == 0) {
    											t3 = true;
    											cube[cube_x,cube_y,cube_z] = 1;
    											front = 2; bottom = 0;
    										}
    										if (!t3) {
    											temp_z+= z_cur.a;
    											z_cur = z_cur.next;
    											cube_z++;
    										}
    									}
    									if (t3) {
    										t2 = true;
    									}
    								}
    								if (y_cur.a == all[i].h && !t2) {
    									z_cur = z.next;
    									cube_z = 1;
    									temp_z = z.a;
    									while (z_cur.next != null && t3 != true) {
    										if (z_cur.a == all[i].w && cube[cube_x,cube_y,cube_z] == 0) {
    											t3 = true;
    											cube[cube_x,cube_y,cube_z] = 1;
    											front = 2; bottom = 4;
    										}
    										if (!t3) {
    											temp_z+= z_cur.a;
    											z_cur = z_cur.next;
    											cube_z++;
    										}
    									}
    									if (t3) {
    										t2 = true;
    									}
    								}
    								if (!t2) {
    									temp_y+= y_cur.a;
    									y_cur = y_cur.next;
    									cube_y++;
    								} else {
    									t1 = true;
    								}
    							}
    						}
    						if (x_cur.a == all[i].h && !t1) {
    							y_cur = y.next;
    							cube_y = 1;
    							temp_y = y.a;
    							while (y_cur.next != null && t2 != true) {
    								if (y_cur.a == all[i].w) {
    									z_cur = z.next;
    									cube_z = 1;
    									temp_z = z.a;
    									while (z_cur.next != null && t3 != true) {
    										if (z_cur.a == all[i].d && cube[cube_x,cube_y,cube_z] == 0) {
    											t3 = true;
    											cube[cube_x,cube_y,cube_z] = 1;
    											front = 4; bottom = 0;
    										}
    										if (!t3) {
    											temp_z+= z_cur.a;
    											z_cur = z_cur.next;
    											cube_z++;
    										}
    									}
    									if (t3) {
    										t2 = true;
    									}
    								}
    								if (y_cur.a == all[i].d && !t2) {
    									z_cur = z.next;
    									cube_z = 1;
    									temp_z = z.a;
    									while (z_cur.next != null && t3 != true) {
    										if (z_cur.a == all[i].w && cube[cube_x,cube_y,cube_z] == 0) {
    											t3 = true;
    											cube[cube_x,cube_y,cube_z] = 1;
    											front = 4; bottom = 2;
    										}
    										if (!t3) {
    											temp_z+= z_cur.a;
    											z_cur = z_cur.next;
    											cube_z++;
    										}
    									}
    									if (t3) {
    										t2 = true;
    									}
    								}
    								if (!t2) {
    									temp_y+= y_cur.a;
    									y_cur = y_cur.next;
    									cube_y++;
    								} else {
    									t1 = true;
    								}
    							}
    						}
    						if (!t1) {
    							temp_x+= x_cur.a;
    							x_cur = x_cur.next;
    							cube_x++;
    						}
    					}
                        Console.WriteLine(position[front] + " " + position[bottom] + " " + temp_x + " " + temp_y + " " +temp_z);
    				}
    			}
    		}
    	}
    }
}