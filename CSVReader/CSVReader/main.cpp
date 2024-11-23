#include <fstream>
#include <iostream>
#include <cstdlib>
#include <cstdio>

#include <vector>
#include <string>
#include <filesystem>
#include <cassert>
#include <chrono>

//	monster		//property	//value
std::vector<std::vector<std::string>> monsterRow{};
std::vector<std::vector<std::string>> intermediateBuffer{}; //for post processing

struct MobData {
	int id{0};
	std::string name{};
};
std::vector<MobData> mobData{};

void LoadMobData() {
	std::ifstream inFile{ "../input/Mob.txt" };
	assert(inFile.is_open());
	std::string buffer{};
	uint64_t lineCount = 0;
	while (getline(inFile, buffer)) {
		std::size_t breakPos = buffer.find('-');
		assert(breakPos != std::string::npos);
		lineCount++;

		MobData mob{};
		const std::string temp = buffer.substr(0, breakPos - 1);
		mob.id = stoi(temp);
		const std::string temp2 = buffer.substr(breakPos + 2);
		mob.name = temp2;
		if (mob.id == 9300203) {

			printf("found him\n");
		}

		mobData.push_back(mob);
	}

}

bool readingGarbage = true;

void DuplicateChecking() {
	for (uint64_t i = 0; i < intermediateBuffer.size(); i++) {
		for (uint64_t j = i + 1; j < intermediateBuffer.size(); j++) {
			if (intermediateBuffer[i][0] == intermediateBuffer[j][0]) {
				intermediateBuffer.erase(intermediateBuffer.begin() + j);
				j--;
			}
		}
	}
}

void PostProcessing(std::string fileName) {
	for (auto& monster : intermediateBuffer) {
		monster.erase(monster.begin(), monster.begin() + 5);
		if (monster[0] == "Respawn time") {
			monster.erase(monster.begin(), monster.begin() + 2);
		}
	}

	DuplicateChecking();

	std::ofstream monsterFile;
	std::ofstream itemFile{ "../output/items.dt" };

	std::vector<std::string> itemBuffer{};

	{
		std::string outputString = "../output/";
		outputString += fileName;
		outputString += ".dt";
		monsterFile.open(outputString);
		assert(itemFile.is_open());
		assert(monsterFile.is_open());

	}
	for (auto& monster : intermediateBuffer) {


		bool foundAnID = false;
		for (auto& mob : mobData) {
			if (monster[0] == mob.name) {
				foundAnID = true;
				monster[0] += ":";
				monster[0] += std::to_string(mob.id);
				break;
			}
		}
		for (auto const& stat : monster) {
			//printf("stat : %s\n", stat.c_str());
			monsterFile.write(stat.c_str(), stat.size());
			monsterFile.put('\n');
		}
		for (uint16_t i = 1; i < monster.size(); i++) {
			itemBuffer.push_back(monster[i]);
		}
		monsterFile.put('\n');
		if (!foundAnID) {
			monsterFile.close();
		}

		assert(foundAnID);
	}

	//removing duplicate items
	for (uint16_t i = 0; i < itemBuffer.size(); i++) {
		for (uint16_t j = i + 1; j < itemBuffer.size(); j++) {
			if (itemBuffer[i] == itemBuffer[j]) {
				itemBuffer.erase(itemBuffer.begin() + j);
				j--;
			}
		}
		itemFile.write(itemBuffer[i].c_str(), itemBuffer[i].size());
		if (i < (itemBuffer.size() - 1)) {
			itemFile.put('\n');
		}
	}
	itemFile.close();
}

void ReadALine(std::ifstream& inFile) {
	char ch;
	uint16_t tabCount = 0;
	std::string buffer{};

	std::vector<uint16_t> tabCountPerMonster{};
	bool inMobs = false;

	while (inFile.get(ch)) {

		if (ch == '\t') {
			if (buffer.size() > 0) {
				if (buffer == "Need Accuracy") {
					for (auto& monster : monsterRow) {
						intermediateBuffer.push_back(std::move(monster));
					}
					monsterRow.clear();
					readingGarbage = true;
					return;
				}
				const uint8_t modAdjustment = (tabCount - 1) % 6;
				const uint8_t tempTabCount = (tabCount - modAdjustment) / 6;
				if (tempTabCount < monsterRow.size()) {
					monsterRow[tempTabCount].push_back(buffer);
				}
				buffer.clear();
			}

			tabCount++;
			buffer.clear();
		}
		else if (ch == '\n') {
			tabCount = 0;
			buffer.clear();
		}
		else {
			buffer.push_back(ch);
		}
	}
}

void ReadADropBlock(std::ifstream& inFile) {
	char ch;
	uint16_t tabCount = 0;
	std::string buffer{};

	while (inFile.get(ch)) {

		if (ch == '\t') {
			if (buffer.size() > 0) {
				if (buffer == "Weapon DEF") {
					for (auto& monster : monsterRow) {
						intermediateBuffer.push_back(std::move(monster));
					}
					monsterRow.clear();
					readingGarbage = true;
					return;
				}
				const uint8_t modAdjustment = (tabCount - 1) % 6;
				const uint8_t tempTabCount = (tabCount - modAdjustment) / 6;
				if (tempTabCount < monsterRow.size()) {
					monsterRow[tempTabCount].push_back(buffer);
				}
				buffer.clear();
			}

			tabCount++;
			buffer.clear();
		}
		else if (ch == '\n') {
			tabCount = 0;
			buffer.clear();
		}
		else {
			buffer.push_back(ch);
		}
	}
}

void ReadTSV(std::string fileName) {
	std::ifstream inFile{};

	{
		std::string inputString = "../input/";
		inputString += fileName;
		inputString += ".tsv";
		inFile.open(inputString);
		assert(inFile.is_open());
	}
	char ch;
	bool lastComma = true;
	std::string buffer{};

	uint16_t whichMonster = 0;
	uint8_t mesoCount = 0;


	while (inFile.get(ch)) {
		if (ch != '\t' && ch != '\n') {
			lastComma = false;
			buffer.push_back(ch);
			continue;
		}
		if (!lastComma) {
			lastComma = true;


			if (buffer.find("Lv.") != std::string::npos) {
				readingGarbage = false;
				printf("buffer : %s\n", buffer.c_str());
				monsterRow.push_back(std::vector<std::string>{buffer});
				buffer.clear();
				continue;
			}

			//might be able to skip this
			//bool foundAreaMatch = false;
			//for (uint8_t i = 0; i < areaNames.size(); i++) {
			//	if (buffer == areaNames[i]) {
			//		buffer.clear();
			//		foundAreaMatch = true;
			//		break;
			//	}
			//}
			//if (foundAreaMatch) {
			//	continue;
			//}

			if (buffer.find("meso") != std::string::npos) {
				mesoCount++;
				if (mesoCount == monsterRow.size()) {
					ReadADropBlock(inFile);
					whichMonster = 0;
					mesoCount = 0;
				}

				buffer.clear();
				continue;
			}
			else if (!readingGarbage) {
				printf("buffer [%d] %s\n", whichMonster, buffer.c_str());
				monsterRow[whichMonster].push_back(buffer);
				whichMonster = (whichMonster + 1) % monsterRow.size();

				buffer.clear();
				continue;
			}

			buffer.clear();
		}
	}
}

int main() {
	printf("current path : %s\n", std::filesystem::current_path().string().c_str());



	//LoadMobData();

	ReadTSV("ElNath");

	ReadTSV("Victoria");
	ReadTSV("Ludi");
	ReadTSV("WorldTour");

	PostProcessing("monsters");

	return 0;
}